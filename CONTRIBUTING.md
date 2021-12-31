# Contributing

Please, check first on the [main page](https://github.com/andrecarlucci/fix#is-this-some-kind-of-evil-magic-how-does-it-work) how does it work.

Here are the steps:

  1. Reproduce the error on your machine, save all the text.
  1. Go to the **Fix.Texts** project and create a new Unit Test under the **ConsoleFixers** folder. Use the other tests as guidance.
  1. Go to the **Fix project** and write your actual class implementing the interface **ICommandFixer** under the folder **CommandFixers**.
  1. Run all tests, **make sure all is green**, be careful as one CommandFixer could be activated before yours, fix that.
  1. **Send a PR!** Don't forget to document how your fixer works in the comments. 

Thanks a lot!
