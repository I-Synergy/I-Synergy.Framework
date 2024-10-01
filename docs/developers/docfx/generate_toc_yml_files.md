# How do i generate toc.yml files from the folder structure?

Creating a toc.yml file from folders containing .md files for DocFX can be streamlined by using the DocFxTocGenerator tool. Here’s a step-by-step guide to help you:

## 1. Install DocFxTocGenerator
Open your terminal or command prompt.\
Run the following command to install the tool globally:
```
dotnet tool install -g DocFxTocGenerator
```

## 2. Generate TOC
Navigate to the root directory of your documentation project.
Run the following command to generate the toc.yml files:
```
DocFxTocGenerator -d "docs/libraries" -o "docs/libraries"
```

## 3. Update docfx.json
Ensure your docfx.json file includes the necessary configuration to recognize the generated TOC files. Add or update the build section as follows:

```json
{
  "build": {
    "content": [
      {
        "files": [
          "**/*.yml",
          "**/*.md"
        ]
      }
    ],
    "tocGeneration": {
      "autoGenerateToc": true,
      "overrideExistingToc": true
    }
  }
}
```