# Fix it!

Fix it is a little pet project that corrects simple errors we do sometimes in console commands.

It is a sameless copy of the amazing project [TheFuck](https://github.com/nvbn/thefuck), but written in c# and made for Windows.

## Examples:

### Fixing git typos
```
C:\dev\personal\Fix>git statu
git: 'statu' is not a git command. See 'git --help'.

The most similar commands are
        status
        stage
        stash

C:\dev\personal\Fix>fix
C:\dev\personal\Fix>git status
On branch main
Your branch is up to date with 'origin/main'.
...
```

## Requirements
- Windows only (sorry!)
- Dotnet 5.0 installed

## Instalation
- Just copy the fix.exe file to your path.

## Switches
- fix.exe -debug : shows debug information.
- fix.exe -plan  : only shows the fixed command but doesn't execute it. 

## License MIT
Project License can be found [here](https://github.com/andrecarlucci/fix/blob/main/LICENSE).
