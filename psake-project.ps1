Include "packages\Hangfire.Build.0.3.3\tools\psake-common.ps1"

Task Default -Depends Pack

Task Test -Depends Compile -Description "Run unit and integration tests." {
    Exec { dotnet test --no-build -c Release "tests\Hangfire.Ninject.Tests" }
}

Task Collect -Depends Test -Description "Copy all artifacts to the build folder." {
    Collect-Assembly "Hangfire.Ninject" "net45"
    Collect-File "LICENSE"
}

Task Pack -Depends Collect -Description "Create NuGet packages and archive files." {
    $version = Get-PackageVersion

    Create-Archive "Hangfire.Ninject-$version"
    Create-Package "Hangfire.Ninject" $version
}