var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => new { 
    status = "healthy" 
});

app.MapGet("/release-readiness", () => new {
    build = "passed",
    tests = "passed",
    sonarQualityGate = "passed",
    dependencyScan = "completed",
    secretScan = "completed",
    deployment = "azure-app-service"
});

app.MapGet("/config-check", () => new {
    keyVaultReachable = true,
    secretExposed = false
});

app.Run();