Storyboarding stuff here: https://drive.google.com/drive/u/2/folders/0B3jqRT8JqflTT1NzZDRRZzcweW8

# Code Bible

## Folders

All scripts must go in the Assets/Scripts folder. Editor scripts must go in the Assets/Scripts/Editor folder. 

## Commenting

All functions and variables need a \<summary\> comment. Summary comments in Unity allow us to hover over text and see the comment in the summary, even in another file. Summary comments look like this:

```
/// <summary>
/// Comment goes here. 
/// </summary>
```

Hitting `\\\` above a function/variable in Unity's Visual Studio will auto-generate a summary comment that you can fill in. 

In-line comments (eg: `// comment`) are fine, but I'd only recommend them if your code is doing something that's difficult to understand at first glance. 

Creating a super complicated long function that's peppered with in-line comments is NOT allowed, that just indicates your code is bad. Divide up the function into smaller parts, and add summary comments to those individual functions. 

## Attributes

Attributes are vastly underutilized by newer Unity programmers, but they make scene editing a LOT easier. 

The easiest attribute to understand is `[Header("Example Grouping")]`. Headers group public variables together under a header in the inspector, making them easier to read/understand. 

Another useful attribute for grouping is `[Tooltip("Comment")]`. Tooltips are the text that will show up in the inspector when you hover over a variable, useful if a user hovers over a variable with a name that is not immediately obvious. 

`[Range(0, 10)]` forces variable to only be a range of numbers, creating a slider in the inspector. 

Finally the custom `[ConditionalHideAttribute("Bool", hideInInspector, inverse, 0, 1)]` will hide a variable in the inspector based on the given "Bool" (name of a bool or getter of a bool in the monobehaviour). hideInInspector determines whether or not the variable is uninteractable or hidden. Inverse will determine whether the variable is hidden if the bool is true or false. Finally the last two digits will allow you to enter a range of numbers since `ConditionalHideAttribute` doesn't play nice with `Range`. 
