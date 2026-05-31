# DevSecOps Security Controls & Governance Report

This document explains the automated security tools running in the pipeline and the rules for when a build should pass or fail.

## 1. Security Tools Inside the Pipeline

* **Code Scanning (SAST):** SonarQube Cloud scans the custom C# code for bugs and bad coding practices before the application is deployed.
* **Package Scanning (SCA):** The native `dotnet list package --vulnerable` command checks open-source NuGet packages to ensure no libraries with known security flaws are imported.
* **Secret Detection:** Gitleaks runs inside a temporary Docker container to scan commits. This catches any accidentally hardcoded passwords, connection strings, or cloud access tokens before they go public.

## 2. Pipeline Failure Rules

### Hard Failures (What Blocks a Deployment)
The pipeline terminates immediately and refuses to deploy code to Azure if:
* Gitleaks detects a plaintext password or API key in the code files.
* SonarQube Cloud fails the automated Quality Gate checks.
* The package scan finds a high-severity vulnerability in the external libraries.

### Warnings (What is Allowed to Pass)
The following issues generate warnings in the logs but do not stop the code deployment:
* Low-risk package bugs where the original creator has not released a fix yet.
* General code layout issues (code smells) that make the code messy but do not cause an active security risk.

## 3. How Issues Are Handled

* **Log Verification:** When a scanner flags an issue, the specific line of code is checked to determine if it is a real danger or a false positive from the scanning tool.
* **Fixing Plan:** High-priority security risks are fixed immediately. Low-risk warnings are added to the project task list to be cleaned up during the next standard code update.