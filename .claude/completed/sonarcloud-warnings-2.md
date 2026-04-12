# SonarCloud Warnings Batch 2 — Fix Tracking

**Status:** DONE

Total: 1,968 warnings · 122 rules · 423 files

## Groups

| Group | Files | Warnings | Status |
|-------|-------|----------|--------|
| A: Mathematics | 153 | 1016 | [x] committed 683870e5 + 844b43a0 |
| B: Core | 50 | 121 | [x] committed bba3fd60 + 0886422c |
| C: UI.Maui / UI.Blazor / AspNetCore.Blazor | 40 | 324 | [x] committed b2c9f8dc + 0886422c |
| D: UI.WPF / UI.WinUI | 49 | 269 | [x] committed 7e4309f9 + 0886422c |
| E: Other src (CQRS, Mvvm, Geography, IO, AspNetCore, Automations, etc.) | 74 | 148 | [x] committed 5aea09af + 0886422c |
| F: Samples / Performance | 36 | 90 | [x] committed e362f49f + 0886422c |

## Fix checklist (by rule)

### S101 — Naming conventions (24 hits, 12 files)
- [x] samples/Sample.Maui/Migrations/001.cs:5
- [x] src/ISynergy.Framework.AspNetCore/Options/CORSOptions.cs:3
- [x] src/ISynergy.Framework.Financial/VAT.cs:6
- [x] src/ISynergy.Framework.Mathematics/Common/SVD.cs:13
- [x] src/ISynergy.Framework.Mathematics/Environments/OctaveEnvironment.cs:348,372
- [x] src/ISynergy.Framework.Mathematics/Environments/REnvironment.cs:47,117
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix3x3.cs:21
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix4x4.cs:20
- [x] src/ISynergy.Framework.Mathematics/Optimization/Unconstrained/BroydenFletcherGoldfarbShanno.LIBLBFGS.cs:74,94,1557,1565
- [x] src/ISynergy.Framework.Mathematics/Wavelets/CDF97.cs:37
- [x] src/ISynergy.Framework.UI.WPF/Services/ClipboardService.cs:18,34
- [x] src/ISynergy.Framework.UI.WPF/Win32/CredentialManager.cs:130

### S107 — Too many parameters (39 hits, 12 files)
- [x] src/ISynergy.Framework.Mathematics/* (various)
- [x] samples/Sample.TokenService/Models/WopiToken.cs:18
- [x] src/ISynergy.Framework.Storage/Abstractions/Services/IStorageService.cs:20
- [x] src/ISynergy.Framework.UI.Blazor/Application/Application.cs:49

### S108 — Empty catch (17 hits, 6 files)
- [x] src/ISynergy.Framework.Mathematics/Decompositions/GeneralizedEigenvalueDecomposition.cs
- [x] src/ISynergy.Framework.Mathematics/Decompositions/JaggedGeneralizedEigenvalueDecomposition.cs
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix.Common.cs:439
- [x] src/ISynergy.Framework.Mathematics/Optimization/Unconstrained/BoundedBroydenFletcherGoldfarbShanno.cs:283
- [x] src/ISynergy.Framework.Mvvm/Commands/Base/BaseAsyncRelayCommand.cs:934
- [x] src/ISynergy.Framework.Mvvm/Commands/DisabledCommand.cs:13,14

### S112 — General exceptions (19 hits)
- [x] src/ISynergy.Framework.Geography/Common/EuclidianCoordinate.cs:58
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix.Product.cs (many lines)

### S1066 — Collapsible if (81 hits, 36 files)
- [x] Core and other libraries (many files)

### S1075 — Hardcoded URIs (10 hits)
- [x] samples/Sample.EventSourcing.Web/Program.cs:20
- [x] samples/Sample.Maui/ViewModels/SlideShowViewModel.cs (multiple)
- [x] src/ISynergy.Framework.Core/Constants/GenericConstants.cs:148

### S1104 — Public fields (7 hits)
- [x] src/ISynergy.Framework.Geography/Global/GlobalPosition.cs:43
- [x] src/ISynergy.Framework.Mathematics/Common/Sort.cs:20
- [x] src/ISynergy.Framework.Mathematics/Environments/OctaveEnvironment.cs:378
- [x] src/ISynergy.Framework.Mathematics/Environments/REnvironment.cs:53,123
- [x] src/ISynergy.Framework.Physics/Units/Unit.cs:15,20

### S1116 — Empty statements (60 hits, 4 files)
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix.Conversions.cs (many)
- [x] src/ISynergy.Framework.Mathematics/Integration/InfiniteAdaptiveGaussKronrod.cs:818,979
- [x] src/ISynergy.Framework.Mathematics/Optimization/Unconstrained/BoundedBroydenFletcherGoldfarbShanno.FORTRAN.cs:3507
- [x] src/ISynergy.Framework.Mathematics/Statistics/Classes.cs:260

### S1117 — Variable shadowing (23 hits)
- [x] Various Mathematics files

### S1118 — Utility classes (21 hits, 16 files)
- [x] Multiple files need static keyword or protected ctor

### S1121 — Assignment in sub-expression (22 hits)
- [x] Various Mathematics files

### S1125 — Boolean literals in expressions (12 hits)
- [x] Various files

### S1133 — Deprecated APIs (48 hits)
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix.Submatrix.cs (many)
- [x] src/ISynergy.Framework.UI.Maui/Services/NavigationService.cs

### S1135 — TODO comments (40 hits, 30 files)
- [x] Various files (resolve or mark NOSONAR)

### S1144 — Unused private members (31 hits)
- [x] Various files

### S1172 — Unused parameters (21 hits)
- [x] Various files

### S1185 — Unnecessary overrides (8 hits)
- [x] src/ISynergy.Framework.UI.Maui/Behaviors/CurrencyEntryBehavior.cs:45
- [x] src/ISynergy.Framework.UI.Maui/Behaviors/DecimalEntryBehavior.cs:45

### S1186 — Empty method bodies (3 hits)
- [x] src/ISynergy.Framework.Core/Extensions/TaskExtensions.cs:82
- [x] src/ISynergy.Framework.UI.WinUI/Controls/Console/Console.xaml.cs:47
- [x] samples/Sample.Maui/ViewModels/ControlsViewModel.cs:115

### S1192 — Duplicate string literals (54 hits)
- [x] Various files

### S1210 — IComparable missing operators (1 hit)
- [x] src/ISynergy.Framework.Core/Points/Point.cs:22

### S1244 — Float equality (22 hits)
- [x] Various files (NOSONAR where intentional)

### S125 — Commented code (94 hits)
- [x] Various files

### S1264 — while(true) to for (12 hits)
- [x] Various files

### S127 — Loop counter modified in body (5 hits)
- [x] Various files

### S1313 — Hardcoded IP (5 hits)
- [x] Already NOSONAR, recheck

### S1450 — Write-only fields (31 hits)
- [x] Various files

### S1481 — Unused local vars (28 hits)
- [x] Various Mathematics files

### S1699 — Virtual call in ctor (14 hits)
- [x] Various Application.cs files

### S1764 — Identical expressions (49 hits)
- [x] src/ISynergy.Framework.Mathematics/Optimization/Unconstrained/BoundedBroydenFletcherGoldfarbShanno.FORTRAN.cs (many)
- [x] src/ISynergy.Framework.UI.WinUI/Services/UpdateService.cs:81

### S1854 — Useless assignments (42 hits)
- [x] Various files

### S1871 — Duplicate branches (12 hits)
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix.Comparisons.cs (many)

### S1905 — Unnecessary casts (10 hits)
- [x] src/ISynergy.Framework.EntityFramework/Extensions/ModelBuilderExtensions.cs
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix.Product.cs

### S1939 — Redundant interface (17 hits)
- [x] Various files

### S1944 — Wrong cast (1 hit)
- [x] src/ISynergy.Framework.UI.WinUI/Behaviors/MultiSelection/MultiSelectionBehavior.cs:69

### S2068 — Hard-coded credentials (9 hits)
- [x] samples/Sample.Maui/Extensions/FluentUI.cs (icons - NOSONAR)
- [x] src/ISynergy.Framework.Core/Constants/GenericConstants.cs:57 (NOSONAR)

### S2094 — Empty classes (2 hits)
- [x] Source generator polyfills

### S2139 — Swallowed exceptions (41 hits)
- [x] Various files

### S2219 — Type check with is (2 hits)
- [x] Various files

### S2223 — Non-readonly static fields (3 hits)
- [x] src/ISynergy.Framework.Mathematics/Common/Sort.cs:20
- [x] src/ISynergy.Framework.Mathematics/Environments/OctaveEnvironment.cs

### S2234 — Wrong argument order (9 hits)
- [x] Various Mathematics files

### S2245 — Weak PRNG (7 hits)
- [x] src/ISynergy.Framework.Mathematics/Random/Generator.cs

### S2292 — Auto-properties (19 hits)
- [x] Various Mathematics files

### S2325 — Can be static (58 hits)
- [x] Various files

### S2326 — Unused type params (3 hits)
- [x] CQRS interfaces, Mathematics

### S2342 — Enum naming (5 hits)
- [x] Various files

### S2368 — Params array (54 hits)
- [x] src/ISynergy.Framework.Mathematics/Matrices/Matrix.Product.cs (many)
- [x] src/ISynergy.Framework.Core/Extensions/BinaryWriterExtensions.cs

### S2436 — Too many generic params (17 hits)
- [x] Various files

### S2583/S2589 — Always true/false (54+16 hits)
- [x] Various files (NOSONAR where WeakAction patterns)

### S2674 — Read ignored (5 hits)
- [x] Various files

### S2933 — Make readonly (22 hits)
- [x] Various files

### S2953 — Dispose confusion (14 hits)
- [x] Various files

### S3010 — Static fields set in instance constructors (2 hits)
- [x] Various files

### S3168 — async void (1 hit)
- [x] Check file

### S3172 — Missing close statement (2 hits)
- [x] Check files

### S3217 — Downcast in foreach (10 hits)
- [x] Various files

### S3220 — Method conflicts (4 hits)
- [x] Various files

### S3236 — Redundant argument (4 hits)
- [x] Various files

### S3241 — Change return type to void (16 hits)
- [x] Various files

### S3246 — Add variance (16 hits)
- [x] Various files

### S3247 — Combined type check and cast (11 hits)
- [x] Various files

### S3260 — Seal private classes (11 hits)
- [x] Various files

### S3267 — foreach to LINQ (44 hits)
- [x] Various files

### S3358 — Nested ternary (15 hits)
- [x] Various Mathematics files

### S3398 — Can be static (3 hits)
- [x] Various files

### S3427 — Method overload (12 hits)
- [x] Various files

### S3442 — Abstract methods in sealed class (3 hits)
- [x] Various files

### S3458 — Switch default first (2 hits)
- [x] Various files

### S3459 — Unread fields (1 hit)
- [x] Check file

### S3604 — Member initialization (2 hits)
- [x] Various files

### S3626 — Redundant jumps (16 hits)
- [x] Various files

### S3776 — Cognitive complexity (84 hits)
- [x] Various files (NOSONAR or refactor)

### S3878 — Params in overrides (4 hits)
- [x] Various files

### S3881 — IDisposable pattern (14 hits)
- [x] Various files

### S3925 — ISerializable pattern (11 hits)
- [x] Exception classes

### S3928 — ArgumentException param name (11 hits)
- [x] Various files

### S3963 — Static ctor redundant (2 hits)
- [x] Various files

### S3971 — Task not awaited (9 hits)
- [x] Various files

### S3973 — Dangling if/else (3 hits)
- [x] Various files

### S3993 — Custom attribute (7 hits)
- [x] Various files

### S4035 — Seal or use IEqualityComparer (5 hits)
- [x] Various files

### S4050 — Operator overloads (1 hit)
- [x] Check file

### S4136 — Method overload order (166 hits)
- [x] Various files

### S4143 — Duplicate dictionary keys (6 hits)
- [x] Various files

### S4144 — Duplicate method bodies (11 hits)
- [x] Various files

### S4200 — C# native type (1 hit)
- [x] Check file

### S4201 — Null checks (4 hits)
- [x] Various files

### S4456 — Parameter check (8 hits)
- [x] Various files

### S4487 — Unused params (31 hits)
- [x] Various files

### S4545 — IComparable misuse (1 hit)
- [x] Check file

### S4581 — StringBuilder (1 hit)
- [x] Check file

### S4663 — XML doc (20 hits)
- [x] Various files

### S5122 — CORS (1 hit)
- [x] Already NOSONAR, recheck

### S5766 — Deserialization (11 hits)
- [x] Various files

### S6418 — Regex timeout (1 hit)
- [x] Check file

### S6561 — Avoid boxing (10 hits)
- [x] Various files

### S6608 — Prefer non-generic (12 hits)
- [x] Various files

### S6610 — StartsWith/EndsWith with single char (11 hits)
- [x] Various files

### S6640 — Unsafe (30 hits)
- [x] Mathematics files

### S6664 — Regex (1 hit)
- [x] Check file

### S6667 — String format (4 hits)
- [x] Various files

### S6670 — Logging (31 hits)
- [x] Various files

### S6672 — Duplicate logging (2 hits)
- [x] Various files

### S6964 — API attributes (2 hits)
- [x] Various files

### S6966 — Await task (10 hits)
- [x] Various files

### S927 — Parameter naming (12 hits)
- [x] Various files
