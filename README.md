Malsys
=================================
Marek's L-systems
---------------------------------
Malsys is highly modular L-system processing library with web interface and gallery.
It supports advanced L-system features like context rewriting or parametric L-systems as well as some unique features like rewriting symbol as another L-system.
Malsys is able to generate output as bitmap image, SVG (vector graphics) image, 3D WebGL scene, ASCII art, or even PNG animation (sorry, no gifs, yet)!


Malsys was done as my bachelors work at Charles university in Prague in 2012.
The thesis is written in English and you can [find it online](http://www.malsys.cz/Home/Thesis) but beware, it was the first larger text I have written in English in my life :)

**Author**: Marek Fiser &lt; malsys@marekfiser.cz &gt;

**Running instance**: http://www.malsys.cz/

**License**: The MIT license, see LICENSE.txt for details.


Main features
-------------
* L-systems are processed with system of connected modules. The way how modules are connected together is described by the user.
  * There are modules for rewriting, for interpreting, drawing, etc. It is fairly simple to create new module without worrying about the rest of the system.
* Currently, Malsys supports five output types: bitmap image, SVG, OBJ (3D model displayed using WebGL), hexagonal ASCII art, and PNG animation.
* Custom written PNG library that is able to generate PNG animations based on [APNG Specification](https://wiki.mozilla.org/APNG_Specification).
* Lexer and parser of the Malsys' source code is generated using F#'s FsLex and FsYacc.
  * Syntax is described as grammar and it is easily extensible.
  * Parsing is pretty fast.
* Compiler was designed with extensibility in mind. It uses Inversion of Control container to allow simple extension by new modules.
* More than 200 unit tests that ensure bug-free L-system engine.

Malsys syntax
-------------
The main idea behind the syntax design was understandability.
Someone who knows L-systems should read the code without problems and someone who do not know L-systems can learn quickly.
This is why I decided to make it keyword driven (a little like SQL).

Malsys supports constants, functions, expressions, and scoping.
Local function or constants hide global ones, parameters hide global variables.

See articles in the [help section](http://www.malsys.cz/Documentation) to learn more about the syntax.
Or just open the formal grammar file that is used to generate the parser (it's in `Malsys.Parsing` project folder :)


Compiling and running
---------------------

Compilation of this project might be a little bit tricky because of the dependency on the F#'s Power pack that has to be installed in the machine.
Also ASP.NET MVC project might give you hard time but you can unload it (and don't compile it).

However, primary intention of releasing this source code was to share some cool solutions and tricks with the world.
Ho give you some idea I will try to list interesting features below.


Brief description and tips
----------------------
Malsys is quite large project.
I think that it is well written and some features are quite cool but it is hard to find them in the code.
Below is short description of all main parts of the project together with tips for good stuff.


### Malsys.Compilers
Modular compiler - a bunch of classes with soft dependencies put together with Autofac dependency resolver.
The main class responsible for the compilation is `CompilersContainer`.

### Malsys.Evaluators
Modular evaluator - the same design as compilerwith main clas `EvaluatorsContainer`.

Usage of F#'s immutable data structures (mainly `FSharpMap`) for evaluation of expressions.
This decision was made to simplify scoping. Saving state is as simple as saving reference.
Any change of state will produce new instance. This feature is often used in the library.
Check out the `ExpressionEvaluatorContext` class.

### Malsys.Processing
All the L-system processing code is in this namespace.
The `ProcessManager` class is the boss here.
It can parse, compile, end evaluate L-systems from string.

### Malsys.Processing.Components
In this namespace are all the default components.
All the juicy stuff about processing of the L-systems is located here.

Out of all the components I would like to point you to the rewriter class `SymbolRewriter`, I think it's pretty cool.
I have not seen implementation out there that handles all the context rules correctly (without depth restrictions).
The only weird decision was to keep L-systems in the string, not as a graph so the graph is created on the fly every time to evaluate context rules efficiently.

Also, the `LsystemInLsystemProcessor` class used to interpret an L-system as a symbol is really neat!

### Malsys.Reflection
Components in Malsys are connected based on the input from user.
Thus, reflection is used to obtain all the information about components.

The cool part is that the doc comments can be obtained, too.
I have created some custom tags in the doc comments of components that are interpreted in special way and can help user.
For example, look at the [documentation about components online](http://www.malsys.cz/Documentation/Predefined/Components) or in my thesis (Appendix J) - all human readable, useful, and automatically generated.

### Malsys.Resources
This namespace contains standard library for the Malsys.
All functions, operators, and other predefined stuff is here.
Creation of new operator of function is as simple as addition of one property to the an appropriate class.
Everything else is automatic (reflection baby! :).
Check out `StdConstants`, `StdOperators`, and `StdFunctions` classes.

### Malsys.SemanticModel
This namespace contains all data structures for compiled and evaluated elements.

### Malsys.SourceCode.Printers
Source code printers are doing the opposite of compilation.
The `CanonicPrinter` class is very crucial because it prints the source code in canonical way.
This is used for storing the L-systems in the DB.

### Malsys.Ast
The abstract syntax tree.
This namespace is in the separated project to avoid circular dependency between `Malsys` and `Malsys.Paring` project.
One could define the AST in the `Malsys.Paring` project but I do not like defining classes in F#.

### Malsys.Parsing
This is the parser of Malsys syntax.
It is written using FsLex and FsYacc tools - the same tools that are used by F# compiler.
FsYacc is LALR parser generator and it is quite simple to write the grammar for it.
The problem with LALR parsers is error recovery that is quite poor in Malsys.

The grammar has around 600 lines and parsing is one-liner.
The hard work is done in compilation step where F# tools generate the lexer and parser.

### Malsys.Common
This namespace is in separate project with stuff that is common to all other projects.
This is again to avoid circular dependencies.

This namespace contains many extension and helper classes.
Check them out, you might find some helpful.
For example, `SplitToLines` functions that handles all three line endings works as a stream (returns `IEnumerable<string>`).
Also, `CountLines` function that works with all three line endings and does not perform any allocations might be useful.

### Malsys.Web DB
Database of Malsys contains 13 tables and it was created using Entity Framework DB first.
Nowadays, I prefer code first approach tough.
Tables about the discussion are currently unused because I have switched to Disqus.

Malsys saves all processed L-sytems to the DB and it also tracks which L-systems were created from which.
So if you copy-paste example from the web and then keep changing it, it is possible to search the whole evolution sequence.
This feature was implemented to allow search for _interesting_ L-systems because in general the longer the chain the more interesting the L-system can be.

![DB scheme of malsys](http://www.malsys.cz/Img/DbScheme.png)

### Malsys.Web.Areas.Documentation
Documentation is written with help of classes in `Malsys.Web.ArticleTools`.
All the articles are written directly as `.cshtml` files.
This is faster and avoids a need for some CMS.

The cool thing is that all examples are automatically processed by the L-system engine!
This dramatically simplifies writing and also there are no mistakes in examples.
Some documentation pages are automatically generated using reflection and custom doc-comments on classes as described in `Malsys.Reflection`.

One hidden feature of the documentation is that it can generate LaTeX!
I have used it to generate some appendices to my thesis.
When you look at `PredefinedController` you can find actions `ComponentsLatex` and `ConfigurationsLatex`.
Those actions should work in the live version of the web.
