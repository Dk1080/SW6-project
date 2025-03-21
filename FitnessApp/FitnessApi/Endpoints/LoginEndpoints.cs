namespace FitnessApi.Endpoints
{
    public static class LoginEndpoints
    {
        public static WebApplication MapLoginEndpoints(this WebApplication app)
        {
            app.MapGet("/Test", () =>
            {
                return "Hello there!!";
            });

            return app;
        }
    }
}
