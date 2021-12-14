using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Allow XML and Json response formate
//NewtonsoftJson let us use PATCH in a correct formate Json that .Net could convert
//IT'S IMPORTANT TO ADD NEWSOFTJSON BEFORE ADD XMLDATACONTRACT. IF YOU INVERT THE POSITIONS SERVER WILL RETURN A XML RESPONSE
builder.Services.AddControllersWithViews(setupAction => 
{
    setupAction.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson(setupAction =>
  {
      setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
  })
  .AddXmlDataContractSerializerFormatters()     
  .ConfigureApiBehaviorOptions(setupAction =>
  {
      setupAction.InvalidModelStateResponseFactory = context =>
      {
          //create a problem details object
          var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
          var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

          //Add additional info not added by default
          problemDetails.Detail = "See the errors field for detail";
          problemDetails.Detail = context.HttpContext.Request.Path;

          //find out which status code to use
          var actionExecutingContext = context as ActionExecutingContext;

          //if there are modelstate errors and all arguments were correctly found/parsed we're dealing with validation errors
          if((context.ModelState.ErrorCount > 0) && (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
          {
              problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
              problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
              problemDetails.Title = "One or more validation errors occurred.";

              return new UnprocessableEntityObjectResult(problemDetails)
              {
                  ContentTypes = {"application/problem+json"}
              };
          }

          //if one of the arguments wasn't correctly found / couldn't be parsed we're dealing with null/unparseble input
          problemDetails.Status = StatusCodes.Status400BadRequest;
          problemDetails.Title = "One or more errors on input occurred.";
          return new BadRequestObjectResult(problemDetails)
          {
              ContentTypes = {"application/problem+json"}
          };
      };      
  });


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
builder.Services.AddDbContext<CourseLibraryContext>(options => options.UseSqlServer(@"Server=DESKTOP-N3UNFRN\SQLEXPRESS;Database=CourseLibraryDB;Trusted_Connection=True;"));

await using var provider = builder.Services.BuildServiceProvider();

using (var scope = provider.CreateAsyncScope())
{
    try
    {
        var context = scope.ServiceProvider.GetService<CourseLibraryContext>();
        //for demo purposes, delete the databse and migrate on startup so we can start with a clean slate
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occured while migrating the database.");
    }
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //Do not show strack trace to user in Production environment
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
        });
    });
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
