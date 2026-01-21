# Shopping Cart Console Application

A C# console application that allows users to manage a shopping cart through a command-line menu.

## Features

- Create an empty shopping cart
- Add products with different quantity types (Units or Kilograms)
- Remove products from the cart
- Display cart contents with total price calculation
- Uses modern C# features: records, switch expressions, and interfaces

## Architecture

### Data Models

- **IQuantity Interface**: Defines the contract for quantity types
  - `UnitQuantity`: Represents items sold by unit count
  - `KilogramQuantity`: Represents items sold by weight

- **Product Record**: Represents a product with name, price per unit, and quantity
  - Uses switch expressions to calculate total price based on quantity type

- **Cart Record**: Represents the shopping cart
  - Manages a list of products
  - Calculates total cart price
  - Provides methods to add/remove products

## How to Run

```bash
cd src/ShoppingCart
dotnet run
```

## Usage

When you run the application, you'll see a menu with the following options:

1. **Add Product**: Add a new product by entering:
   - Product name
   - Price per unit
   - Quantity type (Units or Kilograms)
   - Quantity amount

2. **Remove Product**: Remove a product by selecting its number from the displayed list

3. **Display Cart**: View all products in the cart with individual and total prices

4. **Exit**: Close the application

## Technical Implementation

- **Switch Expressions**: Used for pattern matching when calculating prices based on quantity types
- **Records**: Immutable data structures for Product and Cart
- **Interface**: IQuantity interface with multiple implementations for polymorphism

