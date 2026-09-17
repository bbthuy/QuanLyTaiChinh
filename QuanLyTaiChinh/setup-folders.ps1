# Chay script nay tai thu muc chua file .csproj cua project FinanceWise
# Cach chay: mo PowerShell tai thu muc do, go: .\setup-folders.ps1

$folders = @(
    "Models",
    "Services",
    "Data",
    "Data\Migrations",
    "ViewModels",
    "Views",
    "Controls",
    "Converters",
    "Navigation",
    "Resources",
    "Resources\Icons"
)

foreach ($f in $folders) {
    New-Item -ItemType Directory -Path $f -Force | Out-Null
}

# Cac file .cs thuan (khong phai .xaml) - tao truoc, code sau
$files = @(
    "Models\User.cs",
    "Models\Category.cs",
    "Models\Transaction.cs",
    "Models\Budget.cs",
    "Models\SavingGoal.cs",

    "Services\AuthService.cs",
    "Services\TransactionService.cs",
    "Services\BudgetService.cs",
    "Services\SavingGoalService.cs",
    "Services\AnalyticsService.cs",

    "Data\FinanceWiseDbContext.cs",

    "ViewModels\LoginViewModel.cs",
    "ViewModels\RegisterViewModel.cs",
    "ViewModels\TransactionsViewModel.cs",
    "ViewModels\BudgetsViewModel.cs",
    "ViewModels\SavingGoalsViewModel.cs",
    "ViewModels\DashboardViewModel.cs",
    "ViewModels\AnalyticsViewModel.cs",

    "Navigation\NavigationService.cs"
)

foreach ($file in $files) {
    if (-not (Test-Path $file)) {
        New-Item -ItemType File -Path $file -Force | Out-Null
        Write-Host "Da tao: $file"
    }
}

Write-Host "`nXong! Cac file .cs va thu muc da duoc tao."
Write-Host "Rieng cac file .xaml (Views, Controls, App.xaml, MainWindow.xaml) hay tao bang Visual Studio: chuot phai vao thu muc > Add > Window (WPF) / User Control (WPF)."
