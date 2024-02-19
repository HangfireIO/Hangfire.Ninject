Include "packages\Hangfire.Build.0.4.1\tools\psake-common.ps1"

Task Default -Depends Pack

Task Test -Depends Compile -Description "Run unit and integration tests." {
    Exec { dotnet test --no-build -c release "tests\Hangfire.Ninject.Tests" }
}

Task Collect -Depends Test -Description "Copy all artifacts to the build folder." {
    Collect-Assembly "Hangfire.Ninject" "net45"
    Collect-File "LICENSE"
    Collect-File "README.md"
}

Task Pack -Depends Collect -Description "Create NuGet packages and archive files." {
    $version = Get-PackageVersion

    Create-Package "Hangfire.Ninject" $version
    Create-Archive "Hangfire.Ninject-$version"
}

Task Sign -Depends Pack -Description "Sign artifacts." {
    $version = Get-PackageVersion

    Sign-ArchiveContents "Hangfire.Ninject-$version" "hangfire"
}