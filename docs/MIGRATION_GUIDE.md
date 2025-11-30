# Migration Guide: Azure DevOps to GitHub Repository

## Overview
This guide will help you migrate the I-Synergy.Framework repository from Azure DevOps to GitHub as the primary repository, while maintaining Azure DevOps Pipelines for CI/CD with a **strict PR-based workflow**.

## Prerequisites
- Administrator access to Azure DevOps project
- Administrator access to GitHub repository: `https://github.com/I-Synergy/I-Synergy.Framework`
- Git installed locally
- Access to Azure DevOps variable groups (ACC and PROD)

---

## Part 1: Prepare GitHub Repository

### Step 1.1: Verify GitHub Repository Access
1. Navigate to `https://github.com/I-Synergy/I-Synergy.Framework`
2. Ensure you have **Admin** or **Write** access
3. Verify the repository exists and is accessible

### Step 1.2: Configure Branch Protection Rules (Strongly Recommended)
1. Go to **Settings** → **Branches** in GitHub
2. Click **Add branch protection rule**
3. For **Branch name pattern**, enter: `main`
4. Enable the following:
   - ✅ Require pull request reviews before merging
   - ✅ Require status checks to pass before merging
   - ✅ Require branches to be up to date before merging
   - ✅ Include administrators
5. Click **Create** or **Save changes**

**Note**: This is especially important now since only `main` publishes production packages.

---

## Part 2: Final Push from Azure DevOps to GitHub

### Step 2.1: Push All Branches and Tags to GitHub (One-Time)
Execute this **ONE FINAL TIME** from your local repository or Azure DevOps:

```bash
# Clone from Azure DevOps (if not already cloned)
git clone https://dev.azure.com/i-synergy/I-Synergy.Framework/_git/I-Synergy.Framework
cd I-Synergy.Framework

# Add GitHub as a remote (if not already added)
git remote add github https://github.com/I-Synergy/I-Synergy.Framework.git

# Verify remotes
git remote -v

# Fetch all branches from Azure DevOps
git fetch origin --all

# Push all branches and tags to GitHub
git push github --all
git push github --tags --force

# Verify all branches are on GitHub
git ls-remote github
```

### Step 2.2: Verify GitHub Repository
1. Go to `https://github.com/I-Synergy/I-Synergy.Framework`
2. Verify all branches exist (especially `main` and `development/dotnet10`)
3. Verify all tags are present
4. Check that the latest commits match your Azure DevOps repository

---

## Part 3: Configure Azure DevOps Service Connection

### Step 3.1: Create GitHub Service Connection
1. Open Azure DevOps: `https://dev.azure.com/i-synergy/I-Synergy.Framework`
2. Navigate to **Project Settings** (bottom left)
3. Under **Pipelines**, click **Service connections**
4. Click **New service connection**
5. Select **GitHub** and click **Next**
6. Choose authentication method:
   - **OAuth** (Recommended): Click **Authorize** and sign in to GitHub
   - **Personal Access Token**: Create a PAT in GitHub with `repo` scope
7. Name the connection: `GitHub` (exact name, as referenced in pipeline)
8. Grant access permission to all pipelines
9. Click **Save**

### Step 3.2: Verify Service Connection
1. In **Service connections**, find the newly created `GitHub` connection
2. Click the three dots (⋮) → **Verify**
3. Ensure the connection is successful

---

## Part 4: Update Azure DevOps Pipeline

### Step 4.1: Update Pipeline Repository Source
1. Go to **Pipelines** → **Pipelines**
2. Select your pipeline: `I-Synergy.Framework`
3. Click **Edit**
4. Click the three dots (⋮) at top-right → **Triggers**
5. In the **YAML** tab, click **Get sources**
6. Change the repository source:
   - **Repository type**: GitHub
   - **Connection**: Select the `GitHub` service connection created earlier
   - **Repository**: `I-Synergy/I-Synergy.Framework`
   - **Default branch**: `main`
7. Click **Save**

### Step 4.2: Configure Continuous Integration Triggers
1. In the same **Edit Pipeline** view → **Triggers** tab
2. Under **Continuous integration**, enable:
   - ✅ Enable continuous integration
   - **Branch filters**:
     - Type: **Include**
     - Branches: `main` **only**
3. Under **Path filters** (optional):
   - Type: **Exclude**
   - Paths: `docs/*`, `*.md`
4. Click **Save**

**Important**: Only pushes to `main` trigger production builds. All other branches (including `development/dotnet10`) must go through PRs.

### Step 4.3: Configure Pull Request Triggers
1. In **Triggers** → **Pull request validation**
2. Enable:
   - ✅ Enable pull request validation
   - **Branch filters**:
     - Type: **Include**
     - Branches: `main` (PRs targeting main only)
3. Under **Path filters** (optional):
   - Type: **Exclude**
   - Paths: `docs/*`, `*.md`
4. Click **Save & queue** → **Save**

**Important**: All PRs targeting `main` will be validated with ACC environment and preview packages before merging.

---

## Part 5: Update Variable Groups

### Step 5.1: Clean Up Variable Groups
1. Go to **Pipelines** → **Library**
2. Select **ACC** variable group
3. **Remove** the variable: `GithubPAT` (no longer needed)
4. Click **Save**
5. Repeat for **PROD** variable group

### Step 5.2: Verify Required Variables
Ensure both **ACC** and **PROD** groups still contain:
- `CertPassword` (for NuGet signing)
- `TimestampUrl` (for NuGet signing)
- `BuildConfiguration` (e.g., `Release`)
- `Environment` (e.g., `ACC` or `PROD`)
- Any Azure-related variables for documentation deployment

---

## Part 6: Update Local Development Environment

### Step 6.1: Update Local Git Remote
For all developers working on the project:

```bash
# Navigate to your local repository
cd D:\Projects\I-Synergy\I-Synergy.Framework

# Remove old Azure DevOps remote
git remote remove origin

# Add GitHub as the new origin
git remote add origin https://github.com/I-Synergy/I-Synergy.Framework.git

# Verify the change
git remote -v

# Fetch all branches from GitHub
git fetch origin --all

# Set upstream tracking for current branch
git branch --set-upstream-to=origin/development/dotnet10 development/dotnet10

# Pull latest changes
git pull
```

### Step 6.2: Update Visual Studio Git Settings (if applicable)
1. In Visual Studio, go to **Git** → **Manage Remotes**
2. Remove `origin` pointing to Azure DevOps
3. Add `origin` pointing to `https://github.com/I-Synergy/I-Synergy.Framework.git`
4. Click **OK**

---

## Part 7: Test the Migration

### Step 7.1: Test Direct Push to main (Should Trigger)
**Important**: Only do this if you have permissions and branch protection allows it, or temporarily disable branch protection for testing.

1. In your local repository:
   ```bash
   git checkout main
   git pull
   echo "Test migration - main" >> README.md
   git add README.md
   git commit -m "Test: Verify CI trigger from GitHub (main)"
   git push origin main
   ```
2. Go to Azure DevOps → **Pipelines** → **Pipelines**
3. Verify a new build is triggered automatically
4. Check the build source shows **GitHub**
5. Build should use **PROD** variable group
6. Packages should be **release versions** (no `-preview` suffix)

### Step 7.2: Test Push to development/dotnet10 (Should NOT Trigger)
1. In your local repository:
   ```bash
   git checkout development/dotnet10
   git pull
   echo "Test migration - development" >> README.md
   git add README.md
   git commit -m "Test: Verify no CI trigger from development branch"
   git push origin development/dotnet10
   ```
2. Go to Azure DevOps → **Pipelines** → **Pipelines**
3. **Verify NO build is triggered** (this is expected behavior)
4. This confirms the strict PR-based workflow is working

### Step 7.3: Test PR Trigger (development/dotnet10 → main)
1. Create a feature branch from development:
   ```bash
   git checkout development/dotnet10
   git pull
   git checkout -b test/pr-trigger
   echo "Test PR validation" >> README.md
   git add README.md
   git commit -m "Test: PR validation trigger"
   git push origin test/pr-trigger
   ```
2. Go to GitHub → **Pull requests** → **New pull request**
3. Set base: `main`, compare: `test/pr-trigger`
4. Create the pull request
5. Verify Azure DevOps build is triggered
6. Check the GitHub PR shows the build status
7. Build should use **ACC** variable group (PR build)
8. Version package should have **`-preview`** suffix
9. **Do NOT merge yet** - this is just a test

### Step 7.4: Clean Up Test Changes
1. Close the test PR in GitHub without merging
2. Delete the test branch:
   ```bash
   git checkout development/dotnet10
   git branch -D test/pr-trigger
   git push origin --delete test/pr-trigger
   ```
3. Revert test commits from README:
   ```bash
   # On development/dotnet10
   git reset --hard origin/development/dotnet10
   
   # On main (if you pushed test commit)
   git checkout main
   git reset --hard origin/main
   ```

---

## Part 8: Optional - Archive Azure DevOps Repository

### Step 8.1: Archive or Delete Azure DevOps Repo
**Option A: Archive (Recommended for safety)**
1. Go to **Repos** → **Files** in Azure DevOps
2. Click repository name dropdown
3. Select **Manage repositories**
4. Find your repository
5. Mark as **Read-only** or add a notice in the README

**Option B: Complete Deletion (Use with caution)**
1. Go to **Project Settings** → **Repositories**
2. Select the repository
3. Click **Delete**
4. Type the repository name to confirm
5. Click **Delete**

**Note**: Keep the Azure DevOps repository as read-only for 30-60 days before deletion as a safety measure.

---

## Part 9: Update Documentation and Communication

### Step 9.1: Update Repository Documentation
1. Update `README.md` in GitHub with:
   - New repository URL
   - **Strict PR workflow** requirements
   - All changes to `main` must go through PRs
2. Update any CI/CD badge URLs to point to Azure DevOps pipeline
3. Update contributing guidelines with the new workflow:
   - Development work happens on `development/dotnet10` or feature branches
   - **All changes** to `main` require a PR
   - Only `main` publishes production NuGet packages
   - PR builds create preview packages for testing

### Step 9.2: Notify Team Members
Send a team communication with:
- ✅ New repository URL: `https://github.com/I-Synergy/I-Synergy.Framework`
- ✅ Instructions to update local remotes (see Step 6.1)
- ✅ **NEW WORKFLOW**: 
  - Direct pushes to `main` are **restricted** (branch protection)
  - All changes must go through **PRs targeting `main`**
  - PRs build with ACC environment and create `-preview` packages
  - Only merged PRs to `main` publish production packages
  - Pushes to `development/dotnet10` **do NOT trigger builds**
- ✅ CI/CD remains in Azure DevOps (no change)

---

## Part 10: Post-Migration Checklist

### Verification Checklist
- [ ] GitHub repository is the source of truth
- [ ] All branches migrated to GitHub
- [ ] All tags migrated to GitHub
- [ ] Azure DevOps pipeline pulls from GitHub
- [ ] CI trigger works **only on push to `main`**
- [ ] Push to `development/dotnet10` does **NOT** trigger build (verified)
- [ ] PR validation triggers work (PRs targeting `main`)
- [ ] Build status appears in GitHub PRs
- [ ] Branch protection enabled on `main` in GitHub
- [ ] All developers updated their local remotes
- [ ] All developers notified about new PR-based workflow
- [ ] Variable groups cleaned up (no `GithubPAT`)
- [ ] Azure DevOps repository archived or marked read-only
- [ ] Documentation updated with new workflow
- [ ] Team notified

### Build Trigger Summary (Updated)
**Continuous Integration (Direct Push):**
- Push to `main` → ✅ Build triggered → Uses **PROD** variables → **Release package**
- Push to `development/dotnet10` → ❌ **No build** → Must create PR to `main`
- Push to feature branches → ❌ **No build** → Must create PR to `main`

**Pull Request Validation:**
- PR targeting `main` (from any branch) → ✅ Build triggered → Uses **ACC** variables → **Preview package** (with `-preview` suffix)
- PR targeting other branches → ❌ No build triggered

**Workflow Summary:**
```
feature/development/dotnet10 branch
    ↓ (local development)
  (commit & push) → No CI build
    ↓
  (create PR to main) → PR Validation Build (ACC, preview package)
    ↓
  (PR approved & merged) → CI Build (PROD, release package to NuGet)
```

### Rollback Plan (If Issues Occur)
If you need to rollback:
1. In Azure DevOps pipeline, change repository back to **Azure Repos Git**
2. Restore `development/dotnet10` to CI trigger in `azure-pipelines.yml`
3. Re-add the GitHub push bash task (from git history)
4. Run a build to sync back to GitHub
5. Investigate and resolve issues before re-attempting migration

---

## Troubleshooting

### Issue: Pipeline doesn't trigger on GitHub push to main
**Solution:**
1. Verify GitHub service connection is active
2. Check trigger configuration in pipeline YAML
3. Ensure webhook exists in GitHub:
   - Go to GitHub → **Settings** → **Webhooks**
   - Look for Azure Pipelines webhook
   - Click **Edit** → **Redeliver** test payload

### Issue: Pipeline triggers on push to development/dotnet10 (should not)
**Solution:**
1. Verify `azure-pipelines.yml` trigger section only includes `main`
2. Check Azure DevOps pipeline trigger settings
3. Disable any additional triggers in the UI that might override YAML

### Issue: Build fails with "Repository not found"
**Solution:**
1. Verify service connection has correct permissions
2. Re-authenticate GitHub service connection
3. Ensure repository name is correct: `I-Synergy/I-Synergy.Framework`

### Issue: PR status doesn't show in GitHub
**Solution:**
1. Enable pipeline to report status:
   - Pipeline → **Edit** → **Triggers** → **Pull request validation**
   - Enable **Report build status to GitHub**
2. Grant GitHub App additional permissions in repository settings

### Issue: Wrong variable group used (ACC vs PROD)
**Solution:**
1. Check the `Build.Reason` in build logs
2. For PRs, `Build.Reason` should be `PullRequest` → ACC variables
3. For direct pushes to main, `Build.Reason` should be `IndividualCI` or `BatchedCI` → PROD variables
4. Verify conditional logic in pipeline YAML

### Issue: Developers accidentally push to main directly
**Solution:**
1. Enable branch protection on `main` in GitHub (see Part 1, Step 1.2)
2. Require PR reviews before merging
3. Only repository admins can override (if "Include administrators" is disabled)

### Issue: SonarCloud fails to analyze
**Solution:**
1. Update SonarCloud project settings to recognize GitHub as source
2. Re-authenticate SonarCloud with GitHub
3. Verify project key and organization in `azure-pipelines.yml`

---

## Support and Additional Resources

### Useful Links
- [Azure Pipelines GitHub Integration](https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/github)
- [GitHub Service Connection Setup](https://learn.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints?view=azure-devops&tabs=yaml#github-service-connection)
- [Azure Pipelines YAML Schema](https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/)
- [PR Triggers in Azure Pipelines](https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/github?view=azure-devops&tabs=yaml#pr-triggers)
- [GitHub Branch Protection](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches)

### Contact
For issues or questions, contact your Azure DevOps and GitHub administrators.

---

## Summary of Changes Made

### Modified Files
- **azure-pipelines.yml**
  - Removed the bash task that pushed to GitHub
  - Updated CI trigger to **only `main`** branch (removed `development/dotnet10`)
  - Added PR trigger configuration (only for PRs targeting `main`)
  - Cleaned up comments
  - Fixed duplicate NuGetCommand task in Release stage

### Variables No Longer Needed
- `GithubPAT` - Remove from ACC and PROD variable groups

### New Configuration
- GitHub service connection in Azure DevOps
- Pipeline repository source changed from Azure Repos to GitHub
- **Strict PR-based workflow**: CI triggers only on `main`, all other changes via PR
- PR triggers configured only for PRs targeting `main`
- Branch protection recommended on `main` in GitHub

### Key Workflow Changes
**Before Migration:**
- Azure DevOps was primary repository
- Pushes to both `main` and `development/dotnet10` published packages
- GitHub was a mirror

**After Migration:**
- GitHub is primary repository
- **Only `main`** publishes production packages
- **All branches** (including `development/dotnet10`) require PR to `main`
- PRs are validated with preview packages before merge
- Enforces code review and quality gates

---

**Migration Date**: _[Fill in after completion]_  
**Performed By**: _[Your name]_  
**Status**: _[Success/Rollback/In Progress]_
