# Contributing

Please, check first on the [main page](https://github.com/andrecarlucci/fix#is-this-some-kind-of-evil-magic-how-does-it-work) how does it work.

Here are the steps:

  1. Reproduce the error on your machine, save all the text.
  1. Go to the **Fix.Texts** project and create a new Unit Test under the **ConsoleFixers** folder. Use the other tests as guidance.
  1. Go to the **Fix project** and write your actual class implementing the interface **ICommandFixer** under the folder **CommandFixers**.
  1. Run all tests, **make sure all is green**, be careful as one CommandFixer could be activated before yours, fix that.
  1. **Send a PR!** Don't forget to document how your fixer works in the comments. 

Thanks a lot!

## Let's make one together

For this fixer, I want to fix a DOS command typo.

### 1- Reproduce the error in the console

```
C:\dev\fix>dri /a /b /o
'dri' is not recognized as an internal or external command,
operable program or batch file.

C:\dev\fix>
```

As you you can see, I typed **dri** instead of **dir**. It's been a long day.
I don't want to type, up, home, fix the typo and hit enter again. Let's just use **fix** for this.

### 2- Create a unit test with our desired result

I will add this to the Fix.Tests project:

```
public class DosSimpleTypoTests
{
    [Fact]
    public void Can_fix_simple_dos_typo__dir()
    {
        var consoleBuffer = @"C:\dev\app>dri /a /b /o
'dri' is not recognized as an internal or external command,
operable program or batch file.

C:\dev\app>fix
";
        var lines = consoleBuffer.Split(Environment.NewLine);
        ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

        var manager = SetupTestsHelper.CreateActionManager();
        var fix = manager.GetFix(lines);

        Assert.Equal(nameof(DosSimpleTypo), fix.Author);
        Assert.True(fix.IsFixed);
        Assert.Equal("dir /a /b /o", fix.FixedCommand);
    }
}
```

> Please, **always use "c:\dev\app" as the fake path** in your strings otherwise your tests will mess up with the others when running in paralell!

The test will, of course, fail here as we don't have an implementation. I wish I could use **fix** to fix that too!

### 3- Let's write the real implementation

```
public class DosSimpleTypo : ICommandFixer
{
    public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
    {
        if (!lastCommand.StartsWith("dri"))
        {
            return CommandFix.CantFix();
        }

        for (var i = 0; i < consoleBufferInLines.Length; i++)
        {
            if (consoleBufferInLines[i] == "'dri' is not recognized as an internal or external command,")
            {
                var regex = new Regex(Regex.Escape("dri"));
                var newCommand = regex.Replace(lastCommand, "dir", 1);
                return CommandFix.FixesWith(newCommand);
            }
        }
        return CommandFix.CantFix();
    }
}
```

### 4- Run all the tests!

```
C:\dev\personal\Fix>dotnet test
  Determining projects to restore...
  All projects are up-to-date for restore.
  Fix -> C:\dev\personal\Fix\Fix\bin\Debug\net5.0\win-x64\Fix.dll
  Fix.Tests -> C:\dev\personal\Fix\Fix.Tests\bin\Debug\net5.0\Fix.Tests.dll
Test run for C:\dev\personal\Fix\Fix.Tests\bin\Debug\net5.0\Fix.Tests.dll (.NETCoreApp,Version=v5.0)
Microsoft (R) Test Execution Command Line Tool Version 17.0.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     6, Skipped:     0, Total:     6, Duration: 3 ms - Fix.Tests.dll (net5.0)

C:\dev\personal\Fix>
```

All green \o/

### 5- Send your PR!

There you go!
