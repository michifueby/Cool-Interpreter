# COOL Interpreter Documentation

**Complete Technical Documentation for COOL Language Interpreter**

> A comprehensive guide covering architecture, implementation, and usage of the COOL (Classroom Object-Oriented Language) interpreter built as part of the Compiler Construction course at FH Wiener Neustadt.

---

## 📚 Documentation Overview

This documentation provides a complete reference for understanding, using, and extending the COOL interpreter. It is organized into eight comprehensive sections, progressing from high-level architecture to detailed implementation specifics.

---

## 📖 Table of Contents

### [00. Documentation Index](./00-INDEX.md) 📑
**Start here!** Complete overview of all documentation with quick navigation, prerequisites, and quick start guide.

**Key Topics:**
- Documentation structure
- Reading guide for different audiences
- Quick start tutorial
- Prerequisites and setup

**Audience:** All users

---

### [01. Architecture](./01-ARCHITECTURE.md) 🏗️
High-level system design, component interactions, and architectural patterns.

**Key Topics:**
- Three-phase interpreter architecture
- Component diagram and responsibilities
- Design patterns (Facade, Visitor, Immutable Data)
- Data flow through compilation pipeline

**Audience:** Developers, architects, students learning compiler design

**Diagrams:**
- System architecture overview
- Component interaction flow
- Three-phase pipeline
- Package structure

---

### [02. Parsing](./02-PARSING.md) 📝
Lexical analysis and syntax parsing using ANTLR4.

**Key Topics:**
- ANTLR4 grammar specification
- Lexer and parser generation
- AST construction
- Parse error handling

**Audience:** Compiler developers, language designers

**Diagrams:**
- Parsing workflow
- ANTLR parse tree → AST conversion
- Error recovery flow

---

### [03. Semantic Analysis](./03-SEMANTIC-ANALYSIS.md) 🔍
Type checking, symbol table, and semantic validation.

**Key Topics:**
- Symbol table design
- Inheritance validation
- Type checking rules
- Semantic error detection

**Audience:** Compiler developers, type system researchers

**Diagrams:**
- Semantic analysis pipeline
- Inheritance tree
- Symbol table structure
- Type checking workflow

---

### [04. Runtime](./04-RUNTIME.md) ⚙️
Tree-walk interpreter, object model, and execution engine.

**Key Topics:**
- Tree-walk interpretation
- Object model and memory management
- Built-in classes (Object, IO, Int, String, Bool)
- Method dispatch and dynamic binding

**Audience:** Runtime developers, VM implementers

**Diagrams:**
- Execution flow
- Object model hierarchy
- Method dispatch mechanism
- Environment structure

---

### [05. Diagnostics](./05-DIAGNOSTICS.md) 🩺
Error handling, diagnostic system, and error recovery.

**Key Topics:**
- Diagnostic system design
- Error codes and categories
- Source location tracking
- Error message formatting

**Audience:** Tool developers, IDE integrators

**Diagrams:**
- Diagnostic collection flow
- Error reporting pipeline

---

### [06. Testing](./06-TESTING.md) 🧪
Comprehensive testing strategy and test suite organization.

**Key Topics:**
- Testing philosophy
- Test categories (Parser, Semantic, Runtime, Algorithm)
- File-based test discovery
- Test coverage analysis

**Audience:** QA engineers, test developers

**Diagrams:**
- Test suite organization
- Automated test discovery flow

---

### [07. API Reference](./07-API-REFERENCE.md) 📘
Public API documentation with usage examples.

**Key Topics:**
- `CoolInterpreter` public API
- `InterpretationResult` and result types
- Extension points
- Usage patterns and best practices

**Audience:** API consumers, integration developers

**Examples:**
- Basic usage
- Error handling
- Testing workflows
- Custom output redirection

---

### [08. Implementation Details](./08-IMPLEMENTATION-DETAILS.md) 🔧
Code organization, design decisions, and development guidelines.

**Key Topics:**
- Project structure
- Key design decisions and rationale
- Performance considerations
- Known limitations
- Future enhancements
- Development guidelines

**Audience:** Contributors, maintainers

**Includes:**
- Complete namespace overview
- Design decision justifications
- Performance optimization tips
- Contribution guidelines

---

## 🎯 Quick Navigation by Role

### **Students Learning Compiler Construction**
1. Start: [00. Index](./00-INDEX.md) → [01. Architecture](./01-ARCHITECTURE.md)
2. Follow: [02. Parsing](./02-PARSING.md) → [03. Semantic Analysis](./03-SEMANTIC-ANALYSIS.md) → [04. Runtime](./04-RUNTIME.md)
3. Explore: [05. Diagnostics](./05-DIAGNOSTICS.md) → [06. Testing](./06-TESTING.md)
4. Review: [08. Implementation Details](./08-IMPLEMENTATION-DETAILS.md)

**Focus:** Understand each compiler phase and how they connect

---

### **API Users / Integration Developers**
1. Start: [00. Index](./00-INDEX.md) → [07. API Reference](./07-API-REFERENCE.md)
2. Reference: [05. Diagnostics](./05-DIAGNOSTICS.md) (error handling)
3. Optional: [01. Architecture](./01-ARCHITECTURE.md) (system overview)

**Focus:** Public API, usage patterns, error handling

---

### **Contributors / Maintainers**
1. Start: [00. Index](./00-INDEX.md) → [08. Implementation Details](./08-IMPLEMENTATION-DETAILS.md)
2. Architecture: [01. Architecture](./01-ARCHITECTURE.md)
3. Deep Dive: [02. Parsing](./02-PARSING.md) → [03. Semantic Analysis](./03-SEMANTIC-ANALYSIS.md) → [04. Runtime](./04-RUNTIME.md)
4. Quality: [06. Testing](./06-TESTING.md)

**Focus:** Code organization, design decisions, testing guidelines

---

### **Researchers / Language Designers**
1. Start: [03. Semantic Analysis](./03-SEMANTIC-ANALYSIS.md) (type system)
2. Explore: [04. Runtime](./04-RUNTIME.md) (execution model)
3. Compare: [08. Implementation Details](./08-IMPLEMENTATION-DETAILS.md) (design decisions)

**Focus:** Type system, inheritance, execution model

---

## 🚀 Getting Started in 5 Minutes

### Installation
```bash
# Clone repository
git clone <repository-url>
cd Cool-Interpreter

# Build solution
dotnet build

# Run tests
dotnet test

# Run interpreter
dotnet run --project Cool.Interpreter.Console hello.cool
```

### Your First COOL Program
```cool
class Main inherits IO {
    main(): Object {
        out_string("Hello, COOL!\n")
    };
};
```

### Execute It
```bash
# Save as hello.cool
echo 'class Main inherits IO { main(): Object { out_string("Hello, COOL!\n") }; };' > hello.cool

# Run interpreter
dotnet run --project Cool.Interpreter.Console hello.cool
# Output: Hello, COOL!
```

**Next:** Read [00. Index](./00-INDEX.md) for complete quick start guide

---

## 🎓 Academic Context

**Course:** Compiler Construction (M.INFO.B.24.WS25 COM 68598)  
**Institution:** FH Wiener Neustadt  
**Language:** COOL (Classroom Object-Oriented Language)  
**Date:** January 2026

### Learning Objectives
This interpreter demonstrates:
1. **Lexical Analysis** - Tokenization with ANTLR4
2. **Syntax Analysis** - Context-free grammar parsing
3. **Semantic Analysis** - Type checking and symbol tables
4. **Code Generation** - AST construction
5. **Runtime Execution** - Tree-walk interpretation
6. **Error Handling** - Comprehensive diagnostics

---

## 📊 Project Statistics

- **Total Lines of Code:** ~15,000 (excluding generated files)
- **Source Files:** 100+ files
- **Test Cases:** 255+ automated tests
- **Test Coverage:** Parser (95%), Semantics (90%), Runtime (85%)
- **Supported COOL Features:** 100% of specification
- **Documentation Pages:** 8 comprehensive documents
- **Diagrams:** 20+ Mermaid diagrams

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|-----------|
| **Language** | C# 10.0 |
| **Runtime** | .NET 9.0 |
| **Parser** | ANTLR4 4.6.6 |
| **Testing** | NUnit 3.12.0 |
| **IDE** | Visual Studio / VS Code |
| **Documentation** | Markdown + Mermaid |

---

## 📁 Documentation Files

| File | Size | Purpose | Last Updated |
|------|------|---------|--------------|
| [00-INDEX.md](./00-INDEX.md) | ~5KB | Navigation & Quick Start | Jan 2026 |
| [01-ARCHITECTURE.md](./01-ARCHITECTURE.md) | ~15KB | System Design | Jan 2026 |
| [02-PARSING.md](./02-PARSING.md) | ~12KB | Lexer & Parser | Jan 2026 |
| [03-SEMANTIC-ANALYSIS.md](./03-SEMANTIC-ANALYSIS.md) | ~18KB | Type System | Jan 2026 |
| [04-RUNTIME.md](./04-RUNTIME.md) | ~20KB | Execution Engine | Jan 2026 |
| [05-DIAGNOSTICS.md](./05-DIAGNOSTICS.md) | ~10KB | Error Handling | Jan 2026 |
| [06-TESTING.md](./06-TESTING.md) | ~12KB | Test Strategy | Jan 2026 |
| [07-API-REFERENCE.md](./07-API-REFERENCE.md) | ~15KB | Public API | Jan 2026 |
| [08-IMPLEMENTATION-DETAILS.md](./08-IMPLEMENTATION-DETAILS.md) | ~25KB | Code Deep Dive | Jan 2026 |
| **Total** | **~132KB** | **Complete Documentation** | - |

---

## 🎨 Diagram Legend

Throughout the documentation, you'll find Mermaid diagrams illustrating various concepts:

- **📊 Architecture Diagrams** - System structure and components
- **🔄 Flow Diagrams** - Process flows and state transitions
- **🌳 Tree Diagrams** - Inheritance hierarchies and AST structures
- **📦 Package Diagrams** - Code organization
- **🔗 Sequence Diagrams** - Component interactions

All diagrams are rendered automatically in GitHub, VS Code, and compatible markdown viewers.

---

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Read [08. Implementation Details](./08-IMPLEMENTATION-DETAILS.md) - Development guidelines
2. Check existing issues or create new one
3. Follow coding style guidelines
4. Add tests for new features
5. Update documentation
6. Submit pull request

**Code Style:** See [Development Guidelines](./08-IMPLEMENTATION-DETAILS.md#development-guidelines)  
**Testing:** See [Testing Strategy](./06-TESTING.md)

---

## 📜 License

Copyright (c) FH Wiener Neustadt. All rights reserved.

This project is created for educational purposes as part of the Compiler Construction course.

---

## 👥 Development Team

- **Michael Füby** - Architecture, Semantic Analysis, Documentation
- **Armin Zimmerling** - Runtime Execution, Object Model, Built-in Classes
- **Mahmoud Ibrahim** - Parsing, AST Construction, Diagnostics System

---

## 📞 Support & Contact

**For Questions:**
- Course Instructor: See course materials
- Documentation Issues: Open GitHub issue
- Bug Reports: See [05. Diagnostics](./05-DIAGNOSTICS.md) for error codes

**Resources:**
- [COOL Specification](../M.INFO.B.24.WS25%20COM%2068598%20-%201_20251024_1404/)
- [ANTLR4 Documentation](https://www.antlr.org/)
- [Course Materials](../M.INFO.B.24.WS25%20COM%2068598%20-%201_20251024_1404/)

---

## 🎉 Conclusion

This documentation represents a complete technical reference for the COOL interpreter project. It covers:

✅ **Architecture** - How the system is designed  
✅ **Implementation** - How each component works  
✅ **Usage** - How to use the public API  
✅ **Testing** - How quality is ensured  
✅ **Extension** - How to add new features  

**Ready to start?** Begin with [00. Index](./00-INDEX.md)

---

**Last Updated:** January 2026  
**Version:** 1.0.0  
**Status:** ✅ Complete

---

_Built with ❤️ at FH Wiener Neustadt_
