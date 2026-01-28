# Arkanoid Clone – A Unity Architecture Example

This repository contains a small Arkanoid style game created to demonstrate how a Unity project can be structured in a clean and maintainable way.

The game itself is intentionally simple.  
The main purpose of this project is to share architectural ideas and practical patterns that can be useful in real production environments.

This project is open source and meant to be read, explored, and changed.

---

## Why This Project Exists

Many Unity projects start small and grow without a clear structure.  
Over time this often leads to tightly coupled systems, difficult debugging, and features that are hard to extend.

This project tries to show an alternative approach by applying architectural decisions from the very beginning, even for a simple game.

The goal is not to present a perfect solution, but to share one possible way of organizing a Unity project with care.

---

## General Approach

While building this project, a few principles were kept in mind:

- Systems should have clear responsibilities  
- Dependencies should be explicit  
- Game flow should be easy to follow  
- Data should be separated from logic  
- Editor tools should support the workflow  

These ideas guide the structure more than any specific pattern or library.

---

## Dependency Injection

VContainer is used to manage dependencies.

Instead of creating or searching for objects at runtime, systems receive what they need through injection.  
This helps make relationships between systems clearer and reduces hidden coupling.

All registrations are done in a single LifetimeScope, which makes it easier to understand how the application is composed.

---

## Flow Management

Game flow is handled using a state machine based architecture.

There is a root application state that manages high level states such as:

- Main menu  
- In game  
- Pause  
- End game  

Transitions between states are explicit and event driven.  
Gameplay systems do not control the flow directly.  
They only notify when something has happened.

This keeps flow logic in one place and makes it easier to reason about.

---

## Level Management

Levels are data driven and stored as JSON files.

- Levels are loaded using Addressables  
- A LevelCollection defines the order of levels  
- Level data is separate from gameplay logic  

A custom level editor is included to create and manage levels inside the Unity Editor.  
This allows iteration without touching code and keeps level creation consistent.

---

## Gameplay Systems

Gameplay is divided into small, focused systems.

For example:

- The ball handles only movement and collisions  
- The ball manager handles ball lifecycle  
- The brick manager tracks bricks and score  
- Input is abstracted behind an interface  

Each system does one thing and communicates through events or injected dependencies.

---

## Factories

Object creation is handled through factories.

Factories are responsible for instantiating prefabs and injecting dependencies.  
This keeps creation logic out of gameplay systems and makes it easier to change how objects are created later.

---

## Assembly Definitions

Assembly definition files are used to define boundaries between modules.

This helps:

- Reduce compile times  
- Prevent accidental dependencies  
- Encourage clearer separation of concerns  

They are used here as a learning tool as much as a technical one.

---

## Folder Structure

The folder structure is organized by responsibility rather than by Unity defaults.

Runtime code, editor tools, data, and configuration are separated so that each area stays focused on its purpose.

There is no single correct structure, but this layout aims to stay readable as the project grows.

---

## How To Explore The Project

A suggested way to learn from this repository:

- Run the project and observe the game flow  
- Read the state machine code first  
- Follow how dependencies are registered and injected  
- Look at how levels are created and loaded  
- Make small changes and see how the system reacts  

Understanding comes best by experimenting.

---

## Closing Notes

This project represents one way of approaching Unity architecture.  
It is not meant to be followed blindly or treated as a final answer.

If it helps you think more carefully about structure, responsibilities, and flow, then it has achieved its purpose.

Contributions, questions, and discussions are always welcome.
