# DevSecOps Security Controls and Pipeline Rules

This document explains the security checks used in the Azure DevSecOps Release Gate project and how those checks should affect release decisions.

## 1. Security tools in the pipeline

| Tool | Type | What it checks |
|---|---|---|
| SonarQube Cloud | SAST / code quality | Scans the custom .NET code for bugs, vulnerabilities, maintainability issues, and quality gate status. |
| Gitleaks | Secret scanning | Scans the repository for leaked passwords, tokens, API keys, and connection strings. |
| .NET package scan | SCA / dependency scanning | Checks direct and transitive NuGet packages for known vulnerabilities. |
| Azure Key Vault | Secret management | Stores sensitive values outside the source code and pipeline YAML. |
| Managed Identity | Passwordless Azure identity | Lets the Azure App Service access Key Vault without storing credentials in the app. |
| Application Insights | Monitoring | Tracks requests, failures, response time, and basic runtime behavior. |

## 2. Current demo behavior

| Control | Current behavior |
|---|---|
| SonarQube Cloud quality gate | The pipeline publishes the quality gate result before packaging and deployment. |
| Gitleaks | Runs in onboarding/reporting mode because `continueOnError: true` is currently enabled. This makes secret findings visible without stopping the whole demo pipeline. |
| Dependency scan | Runs during the build stage using `dotnet list package --vulnerable --include-transitive`. |
| Key Vault check | The application checks whether the Key Vault-backed secret is reachable, but it never prints the secret value. |
| Application Insights | Shows traffic, failed requests, response time, and request volume after deployment. |

## 3. Strict production release rules

In a stricter production setup, the deployment should be blocked when:

- Gitleaks detects a real plaintext password, API key, token, or connection string.
- SonarQube Cloud fails the quality gate.
- A high-severity dependency vulnerability is found and no risk exception has been approved.
- The build or test phase fails.
- The artifact cannot be created or downloaded.
- The deployment task fails.

## 4. Warning-only cases

Some findings should be tracked but may not need to block a release immediately:

- Low-risk maintainability issues.
- Low-severity dependency vulnerabilities without an available fix.
- Findings confirmed as false positives after checking the code and logs.

## 5. How findings should be handled

1. Check the pipeline logs and confirm exactly which tool raised the issue.
2. Identify whether the finding is real or a false positive.
3. For real high-risk issues, fix before release or escalate for approval.
4. For accepted temporary risks, document the reason, owner, and expiry date.
5. Do not disable the scanner just to make the pipeline green.
