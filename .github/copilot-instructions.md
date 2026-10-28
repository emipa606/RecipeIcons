# GitHub Copilot Instructions for Recipe Icons (Continued) Mod

## Mod Overview and Purpose
Recipe Icons (Continued) is an update of the AUTOMATICs mod for RimWorld. The mod's primary goal is to enhance user experience by adding icons to various menus and tooltips in the game. These icons provide quick visual identification of items and recipes, making it easier for players to manage their colonies and perform actions in the game.

## Key Features and Systems
1. **Tooltips with Recipe Information**: Clicking on an icon will display a detailed information dialog for the related item or recipe.
2. **Icons in Thing Filters**: Enhances the "Add Bill" menu and other filters by adding icons, making it easier to identify items.
3. **Icons for Surgery Icons**: Provides visual cues for surgical operations in the health tab.
4. **Support for Complex Items with Cutout Shader**: Allows for more intricate and detailed icons, particularly useful for body parts and other complex items.

## Coding Patterns and Conventions
- **Modular Architecture**: The mod is structured into different classes, each responsible for a specific feature (e.g., `Icon.cs`, `TextLayout.cs`, `Settings.cs`).
- **Harmony Patching**: Utilizes Harmony to patch existing RimWorld code, ensuring that the mod integrates smoothly with the game's existing systems.
- **Clean and Readable Code**: Emphasizes readability and maintainability, making it easier for developers to contribute to or understand the mod's codebase.

## XML Integration
- **About.xml**: Contains metadata about the mod, such as its name, description, and version.
- **XML Defs**: Although not explicitly mentioned in the summary, XML definitions are used to integrate new features into the game's existing systems. For example, new icons or tooltips might be defined in XML files.

## Harmony Patching
- **Patching Existing Code**: The mod uses Harmony to patch existing RimWorld methods, such as `BillUtility_LayoutTooltip.cs` and `FloatMenu_SizeMode.cs`. This allows the mod to modify the behavior of these methods without altering the original codebase.
- **Prefix and Postfix Patches**: Uses Harmony's `Prefix` and `Postfix` attributes to inject custom logic before and after the original method execution.

## Suggestions for Copilot
- **Generate XML Definitions**: Help in creating new XML definitions for icons or tooltips.
- **Implement Harmony Patches**: Assist in implementing new Harmony patches to extend the mod's functionality.
- **Enhance UI Elements**: Suggest improvements or new UI elements to further enhance the user experience.
- **Debugging and Error Handling**: Provide suggestions for debugging and handling errors, especially related to shader integration and complex item rendering.

By following these guidelines, you can contribute effectively to the Recipe Icons (Continued) mod and help enhance the overall user experience in RimWorld.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

