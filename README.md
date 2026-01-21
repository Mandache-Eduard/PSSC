# Shopping Cart Console Application

A C# console application implementing a complete type system for order management with state transitions.

## Overview

This project demonstrates advanced C# concepts including:
- **Domain-Driven Design** with separation of concerns
- **State Pattern** using interfaces and records
- **Immutable Value Objects** with validation
- **Entity Types** with identity
- **Switch Expressions** for pattern matching
- **Type Safety** and compile-time guarantees

## Features

- Create an empty shopping cart for a customer
- Add products with validated product codes and quantities
- Transition cart through different states (Empty → Unvalidated → Validated → Paid)
- Display cart contents at any state
- Process payments with shipping address
- Customer management with identity

## Architecture

### Domain Structure

```
Domain/
├── Exceptions/
│   ├── InvalidProductCodeException
│   ├── InvalidQuantityException
│   └── InvalidAddressException
└── Models/
    ├── Value Objects (Immutable):
    │   ├── ProductCode (format: XX1234)
    │   ├── Quantity (positive decimal)
    │   ├── Address (street, city, postal code, country)
    │   └── ProductItem (record combining above)
    ├── Entity Types (with Identity):
    │   ├── Customer (Guid, Name, Email, Address)
    │   └── ShoppingCartEntity (Guid, Customer, State)
    └── State Interface:
        └── ICart with implementations:
            ├── EmptyCart
            ├── UnvalidatedCart (contains products)
            ├── ValidatedCart (validated, with total)
            └── PaidCart (paid, with date & address)
```

### Key Design Patterns

#### 1. **State Pattern**
The shopping cart can be in one of four states, each represented by a record implementing `ICart`:
- `EmptyCart`: Initial state, no products
- `UnvalidatedCart`: Contains products but not validated
- `ValidatedCart`: Validated and ready for payment
- `PaidCart`: Payment processed with shipping information

#### 2. **Value Objects**
Immutable types with validation:
- **ProductCode**: Validates format (2 letters + 4 digits)
- **Quantity**: Ensures positive values
- **Address**: Validates all fields are non-empty

#### 3. **Entity Types**
Types with identity (Guid):
- **Customer**: Mutable entity with identity, can update name and shipping address
- **ShoppingCartEntity**: Manages cart state transitions

#### 4. **Switch Expressions**
Used throughout for:
- State transition logic
- Display logic based on cart state
- Price calculations

## How to Run

```bash
cd src/ShoppingCart
dotnet run
```

## Usage

### Initial Setup
1. Enter your name and email to create a customer account
2. An empty cart is automatically created

### Menu Options

1. **Add Product to Cart**
   - Enter product code (format: XX1234, e.g., AB1234)
   - Enter product name
   - Enter price per unit
   - Enter quantity
   - Product is added and cart transitions to Unvalidated state

2. **Display Cart Contents**
   - Shows all products with prices
   - Display format changes based on cart state
   - Shows total price

3. **Transition Cart State**
   - From Unvalidated → Validate Cart
   - From Validated → Process Payment (requires shipping address)
   - Uses switch expressions to determine available transitions

4. **Display Customer Info**
   - Shows customer details and shipping address (if set)

5. **Exit**
   - Close the application

## Example Flow

```
Empty Cart → Add Products → Unvalidated Cart → Validate → Validated Cart → Pay → Paid Cart
```

## Technical Implementation Details

### Switch Expressions
```csharp
// State-based logic
string description = cart.CurrentState switch
{
    EmptyCart => "Cart is empty",
    UnvalidatedCart u => $"Cart has {u.Products.Count} products",
    ValidatedCart v => $"Total: {v.TotalPrice:C}",
    PaidCart p => $"Paid on {p.PaidDate}",
    _ => "Unknown"
};
```

### Immutable Records
```csharp
public record ProductItem(ProductCode Code, string Name, decimal Price, Quantity Quantity)
{
    public decimal TotalPrice => Price * Quantity.Value;
}
```

### State Transitions
```csharp
// Only valid transitions are allowed
public void ValidateCart()
{
    if (CurrentState is UnvalidatedCart unvalidated)
    {
        decimal total = unvalidated.Products.Sum(p => p.TotalPrice);
        CurrentState = new ValidatedCart(unvalidated.Products, total);
    }
    else throw new InvalidOperationException("Cannot validate cart in current state");
}
```

## Learning Points from L2 Examples

The code organization follows patterns from `Examples/L2`:

1. **Interface-based State Pattern**: Like `IExam` with multiple state implementations
2. **Record Types**: Using records for immutable state representations
3. **Value Objects with Validation**: Like `Grade` and `StudentRegistrationNumber`
4. **Domain Folder Structure**: Separating Models and Exceptions
5. **Switch Expressions**: Pattern matching for state handling
6. **Unvalidated vs Validated Types**: Separating concerns by validation state

