# Threat Modeling (STRIDE)
STRIDE threat modelling: assets, trust boundaries and the threat catalogue. Load this when threat-modelling a design.

## Threat Modeling (STRIDE)

### STRIDE Methodology

- **S**poofing - Can an attacker impersonate a user or system?
- **T**ampering - Can an attacker modify data in transit or at rest?
- **R**epudiation - Can a user deny performing an action?
- **I**nformation Disclosure - Can sensitive data be exposed?
- **D**enial of Service - Can the system be made unavailable?
- **E**levation of Privilege - Can a user gain unauthorized access?

### Example Threat Model for Budget API

```markdown
# Budget API Threat Model

### Assets
- User data (email, password hashes)
- Budget data (amounts, categories)
- Authentication tokens

### Trust Boundaries
- Client ↔ API Gateway
- API Gateway ↔ Microservices
- Microservices ↔ Database

### Threats

### Spoofing
- **Threat:** Attacker steals JWT token and impersonates user
- **Mitigation:** Short-lived tokens, HTTPS only, secure storage

### Tampering
- **Threat:** Attacker modifies budget amounts in transit
- **Mitigation:** HTTPS, request signing, integrity checks

### Repudiation
- **Threat:** User denies creating a budget
- **Mitigation:** Audit logging with timestamps, correlation IDs

### Information Disclosure
- **Threat:** Attacker accesses other users' budgets
- **Mitigation:** Authorization checks, resource-based access control

### Denial of Service
- **Threat:** Attacker floods API with requests
- **Mitigation:** Rate limiting, throttling, circuit breakers

### Elevation of Privilege
- **Threat:** Regular user accesses admin functions
- **Mitigation:** Role-based access control, authorization policies
```

---
