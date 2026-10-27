# Static Analysis & Security Code Review
Running SAST tooling and the security-focused code-review checklist. Load this when reviewing a change for security defects.

## Static Application Security Testing (SAST)

### Security Code Analysis

```xml
<!-- File: Directory.Build.props -->

<Project>
  <PropertyGroup>
    <!-- Enable security code analysis -->
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisMode>All</AnalysisMode>
    <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>

    <!-- Security analyzers -->
    <RunAnalyzersDuringBuild>true</RunAnalyzersDuringBuild>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0" />
    <PackageReference Include="SecurityCodeScan.VS2019" Version="5.6.7" />
  </ItemGroup>
</Project>
```

---

## Security Code Review Checklist

### Input Validation
- [ ] All user inputs validated at API boundary
- [ ] Data annotations used for simple validation
- [ ] IValidatableObject used for complex validation
- [ ] Input sanitized to prevent injection
- [ ] File uploads validated (type, size, content)

### Output Encoding
- [ ] All output encoded (HTML, JavaScript, URL)
- [ ] Razor automatically encodes output
- [ ] No use of @Html.Raw with user input

### SQL Injection
- [ ] EF Core used for all database access
- [ ] No raw SQL string concatenation
- [ ] Parameterized queries for raw SQL

### XSS Prevention
- [ ] Content Security Policy configured
- [ ] Security headers set
- [ ] Output encoding applied

### CSRF Protection
- [ ] Anti-forgery tokens used
- [ ] SameSite cookie attribute set
- [ ] State-changing operations use POST/PUT/DELETE

### Authentication
- [ ] OpenIddict or ASP.NET Identity used
- [ ] JWT tokens validated
- [ ] Short-lived access tokens (< 30 minutes)
- [ ] Refresh tokens implemented
- [ ] HTTPS required

### Password Security
- [ ] Passwords hashed with BCrypt or Argon2
- [ ] Never stored in plaintext
- [ ] Minimum password strength requirements
- [ ] Password reset uses secure tokens

### Secrets Management
- [ ] No secrets hard-coded in code
- [ ] Azure Key Vault used for production
- [ ] Secrets not in source control
- [ ] Environment-specific secrets

### Authorization
- [ ] Authorization checked at every endpoint
- [ ] Principle of least privilege applied
- [ ] Resource-based authorization where needed
- [ ] Fail closed on errors

### Logging
- [ ] No PII in logs
- [ ] No passwords, tokens, or secrets logged
- [ ] Security events logged
- [ ] Structured logging used

### Dependencies
- [ ] NuGet audit enabled
- [ ] Dependabot configured
- [ ] Regular updates to dependencies
- [ ] No known vulnerabilities

---

