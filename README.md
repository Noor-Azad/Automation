# Tedile Playwright Automation

End-to-end automation for **Tedile** using **C# + Microsoft Playwright + xUnit**.

This repository intentionally replaces the previous Java/Selenium/TestNG proof-of-concept and its generated `target`, `test-output`, `allure-results`, and screenshot artifacts.

The new framework follows the supplied C# reference architecture: central BaseTest lifecycle, reusable browser fixture, Page Object Model, configuration layer, data providers, API clients, logging, Playwright traces/screenshots, and CI execution.

Full setup and usage instructions are included as the framework files are added in this branch.
