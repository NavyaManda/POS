/**
 * REUSABLE MENUSERVICE - RESTAURANT TYPE EXAMPLES
 * 
 * This MenuService is designed to be highly customizable and work for different restaurant types:
 * - Pizza Restaurants (with toppings, sizes, crusts)
 * - Indian Biryani Restaurants (with spice levels, sides, extras)
 * - Breakfast Restaurants (with combinations, dietary options)
 * - And many more...
 */

// ============================================
// EXAMPLE 1: PIZZA RESTAURANT SETUP
// ============================================

POST /api/v1/restaurant-config
{
  "restaurantId": "pizza_place_001",
  "restaurantName": "Pizza Palace",
  "restaurantType": "Pizza",
  "cuisineType": "Italian",
  "description": "Premium Italian Pizza Restaurant",
  "operatingHoursStart": "11:00:00",
  "operatingHoursEnd": "23:00:00",
  "currencyCode": "USD"
}
// Returns: RestaurantConfigId = 1

// Create categories
POST /api/v1/restaurants/1/categories
{
  "name": "Pizzas",
  "description": "All types of pizzas",
  "displayOrder": 1
}
// Returns: CategoryId = 1

POST /api/v1/restaurants/1/categories
{
  "name": "Appetizers",
  "description": "Starters",
  "displayOrder": 2
}
// Returns: CategoryId = 2

// Create subcategories for better organization
POST /api/v1/categories/1/subcategories
{
  "name": "Vegetarian Pizzas",
  "description": "Meat-free pizzas",
  "displayOrder": 1
}
// Returns: SubcategoryId = 1

POST /api/v1/categories/1/subcategories
{
  "name": "Meat Pizzas",
  "description": "Pizzas with meat",
  "displayOrder": 2
}
// Returns: SubcategoryId = 2

// Create menu item - Margherita Pizza
POST /api/v1/restaurants/1/menu-items
{
  "name": "Margherita Pizza",
  "description": "Classic pizza with fresh mozzarella and basil",
  "shortDescription": "Fresh mozzarella & basil",
  "basePrice": 12.99,
  "categoryId": 1,
  "subcategoryId": 1,
  "isAvailable": true,
  "imageUrl": "https://images.example.com/margherita.jpg",
  "calories": 250,
  "protein": 10,
  "carbohydrates": 35,
  "fat": 8,
  "isVegetarian": true,
  "supportSpiceLevel": false,
  "preparationTimeMinutes": 15
}
// Returns: MenuItemId = 1

// Add size variants for Margherita Pizza
POST /api/v1/menu-items/1/variants
{
  "variantType": "Size",
  "variantName": "Small",
  "description": "8-inch pizza",
  "priceModifier": 0,
  "isDefault": true,
  "displayOrder": 1
}

POST /api/v1/menu-items/1/variants
{
  "variantType": "Size",
  "variantName": "Medium",
  "description": "10-inch pizza",
  "priceModifier": 2.00,
  "isDefault": false,
  "displayOrder": 2
}

POST /api/v1/menu-items/1/variants
{
  "variantType": "Size",
  "variantName": "Large",
  "description": "12-inch pizza",
  "priceModifier": 4.00,
  "isDefault": false,
  "displayOrder": 3
}

// Add crust options for Margherita Pizza
POST /api/v1/menu-items/1/customization-groups
{
  "groupName": "Crust Type",
  "selectionType": "SingleSelect",
  "minimumSelections": 1,
  "maximumSelections": 1,
  "isRequired": true,
  "displayOrder": 1,
  "options": [
    {
      "optionName": "Thin Crust",
      "additionalPrice": 0,
      "isDefault": true,
      "displayOrder": 1
    },
    {
      "optionName": "Regular Crust",
      "additionalPrice": 0,
      "isDefault": false,
      "displayOrder": 2
    },
    {
      "optionName": "Stuffed Crust",
      "additionalPrice": 1.50,
      "isDefault": false,
      "displayOrder": 3
    }
  ]
}

// Add extra toppings
POST /api/v1/menu-items/1/customization-groups
{
  "groupName": "Extra Toppings",
  "selectionType": "MultiSelect",
  "minimumSelections": 0,
  "maximumSelections": 5,
  "isRequired": false,
  "displayOrder": 2,
  "options": [
    {
      "optionName": "Extra Cheese",
      "additionalPrice": 1.00,
      "additionalCalories": 50,
      "displayOrder": 1
    },
    {
      "optionName": "Pepperoni",
      "additionalPrice": 1.50,
      "additionalCalories": 80,
      "displayOrder": 2
    },
    {
      "optionName": "Mushrooms",
      "additionalPrice": 0.75,
      "additionalCalories": 20,
      "displayOrder": 3
    },
    {
      "optionName": "Onions",
      "additionalPrice": 0.50,
      "additionalCalories": 10,
      "displayOrder": 4
    },
    {
      "optionName": "Bell Peppers",
      "additionalPrice": 0.75,
      "additionalCalories": 15,
      "displayOrder": 5
    }
  ]
}

// Create a combo deal
POST /api/v1/restaurants/1/combo-deals
{
  "dealName": "Family Pack Pizza",
  "description": "1 Large Pizza + 2 sides + drinks",
  "comboPrice": 29.99,
  "isAvailable": true,
  "items": [
    {
      "menuItemId": 1,
      "quantity": 1,
      "isInterchangeable": false
    },
    {
      "menuItemId": 3,
      "quantity": 2,
      "isInterchangeable": true,
      "interchangeableGroup": "Sides"
    },
    {
      "menuItemId": 5,
      "quantity": 2,
      "isInterchangeable": true,
      "interchangeableGroup": "Beverages"
    }
  ]
}

// ============================================
// EXAMPLE 2: INDIAN BIRYANI RESTAURANT SETUP
// ============================================

POST /api/v1/restaurant-config
{
  "restaurantId": "biryani_house_001",
  "restaurantName": "Biryani House",
  "restaurantType": "Biryani",
  "cuisineType": "Indian",
  "description": "Authentic Indian Biryani Restaurant",
  "operatingHoursStart": "11:30:00",
  "operatingHoursEnd": "22:00:00",
  "currencyCode": "USD"
}
// Returns: RestaurantConfigId = 2

// Create categories for Indian restaurant
POST /api/v1/restaurants/2/categories
{
  "name": "Biryani",
  "description": "Aromatic rice dishes",
  "displayOrder": 1
}
// Returns: CategoryId = 3

POST /api/v1/restaurants/2/categories
{
  "name": "Appetizers",
  "description": "Starters",
  "displayOrder": 2
}
// Returns: CategoryId = 4

POST /api/v1/restaurants/2/categories
{
  "name": "Breads",
  "description": "Indian breads",
  "displayOrder": 3
}
// Returns: CategoryId = 5

// Create subcategories
POST /api/v1/categories/3/subcategories
{
  "name": "Chicken Biryani",
  "displayOrder": 1
}
// Returns: SubcategoryId = 3

POST /api/v1/categories/3/subcategories
{
  "name": "Lamb Biryani",
  "displayOrder": 2
}
// Returns: SubcategoryId = 4

POST /api/v1/categories/3/subcategories
{
  "name": "Vegetable Biryani",
  "displayOrder": 3
}
// Returns: SubcategoryId = 5

// Create menu item - Chicken Biryani with spice customization
POST /api/v1/restaurants/2/menu-items
{
  "name": "Hyderabadi Chicken Biryani",
  "description": "Authentic Hyderabadi biryani with tender chicken",
  "shortDescription": "Hyderabadi style chicken biryani",
  "basePrice": 8.99,
  "categoryId": 3,
  "subcategoryId": 3,
  "isAvailable": true,
  "calories": 350,
  "protein": 25,
  "carbohydrates": 45,
  "fat": 12,
  "isVegetarian": false,
  "isGlutenFree": true,
  "supportSpiceLevel": true,
  "defaultSpiceLevel": "Medium",
  "preparationTimeMinutes": 25
}
// Returns: MenuItemId = 10

// Add portion variants
POST /api/v1/menu-items/10/variants
{
  "variantType": "Portion",
  "variantName": "Half",
  "description": "500g serving",
  "priceModifier": 0,
  "isDefault": true,
  "displayOrder": 1
}

POST /api/v1/menu-items/10/variants
{
  "variantType": "Portion",
  "variantName": "Full",
  "description": "1kg serving",
  "priceModifier": 3.00,
  "displayOrder": 2
}

POST /api/v1/menu-items/10/variants
{
  "variantType": "Portion",
  "variantName": "Family Pack",
  "description": "1.5kg serving",
  "priceModifier": 5.00,
  "displayOrder": 3
}

// Add spice level customization
POST /api/v1/menu-items/10/customization-groups
{
  "groupName": "Spice Level",
  "selectionType": "SingleSelect",
  "minimumSelections": 1,
  "maximumSelections": 1,
  "isRequired": true,
  "displayOrder": 1,
  "options": [
    {
      "optionName": "Mild",
      "additionalPrice": 0,
      "displayOrder": 1
    },
    {
      "optionName": "Medium",
      "additionalPrice": 0,
      "isDefault": true,
      "displayOrder": 2
    },
    {
      "optionName": "Hot",
      "additionalPrice": 0,
      "displayOrder": 3
    },
    {
      "optionName": "Very Hot",
      "additionalPrice": 0,
      "displayOrder": 4
    }
  ]
}

// Add side options
POST /api/v1/menu-items/10/customization-groups
{
  "groupName": "Add Sides",
  "selectionType": "MultiSelect",
  "minimumSelections": 0,
  "maximumSelections": 3,
  "isRequired": false,
  "displayOrder": 2,
  "options": [
    {
      "optionName": "Raita (Yogurt)",
      "additionalPrice": 1.00,
      "additionalCalories": 80,
      "displayOrder": 1
    },
    {
      "optionName": "Pickle",
      "additionalPrice": 0.50,
      "additionalCalories": 20,
      "displayOrder": 2
    },
    {
      "optionName": "Papad",
      "additionalPrice": 0.75,
      "additionalCalories": 100,
      "displayOrder": 3
    },
    {
      "optionName": "Extra Rice",
      "additionalPrice": 2.00,
      "additionalCalories": 200,
      "displayOrder": 4
    }
  ]
}

// Bundle pricing for quantity discounts
POST /api/v1/menu-items/10/bundle-prices
{
  "minimumQuantity": 3,
  "unitPrice": 8.00,
  "discountPercentage": 12
}

POST /api/v1/menu-items/10/bundle-prices
{
  "minimumQuantity": 5,
  "unitPrice": 7.50,
  "discountPercentage": 17
}

// ============================================
// EXAMPLE 3: BREAKFAST RESTAURANT SETUP
// ============================================

POST /api/v1/restaurant-config
{
  "restaurantId": "breakfast_café_001",
  "restaurantName": "Breakfast Café",
  "restaurantType": "Breakfast",
  "cuisineType": "Continental",
  "description": "All-day breakfast restaurant",
  "operatingHoursStart": "06:00:00",
  "operatingHoursEnd": "14:00:00",
  "currencyCode": "USD"
}
// Returns: RestaurantConfigId = 3

// Create breakfast categories
POST /api/v1/restaurants/3/categories
{
  "name": "Continental Breakfast",
  "displayOrder": 1
}

POST /api/v1/restaurants/3/categories
{
  "name": "Indian Breakfast",
  "displayOrder": 2
}

// Breakfast item with dietary customizations
POST /api/v1/restaurants/3/menu-items
{
  "name": "Power Breakfast Combo",
  "description": "Choose your bread + protein + beverage",
  "basePrice": 9.99,
  "categoryId": 10,
  "isAvailable": true,
  "calories": 450,
  "protein": 20,
  "carbohydrates": 50,
  "fat": 15,
  "isVegetarian": false,
  "isGlutenFree": false,
  "preparationTimeMinutes": 10
}
// Returns: MenuItemId = 20

// Add bread options
POST /api/v1/menu-items/20/customization-groups
{
  "groupName": "Choose Bread",
  "selectionType": "SingleSelect",
  "minimumSelections": 1,
  "maximumSelections": 1,
  "isRequired": true,
  "displayOrder": 1,
  "options": [
    {
      "optionName": "Whole Wheat Toast",
      "additionalPrice": 0,
      "isDefault": true,
      "displayOrder": 1
    },
    {
      "optionName": "White Toast",
      "additionalPrice": 0,
      "displayOrder": 2
    },
    {
      "optionName": "Bagel",
      "additionalPrice": 0.50,
      "displayOrder": 3
    },
    {
      "optionName": "Croissant",
      "additionalPrice": 1.00,
      "displayOrder": 4
    }
  ]
}

// Add protein options
POST /api/v1/menu-items/20/customization-groups
{
  "groupName": "Choose Protein",
  "selectionType": "SingleSelect",
  "minimumSelections": 1,
  "maximumSelections": 1,
  "isRequired": true,
  "displayOrder": 2,
  "options": [
    {
      "optionName": "Scrambled Eggs",
      "additionalPrice": 0,
      "isDefault": true,
      "displayOrder": 1
    },
    {
      "optionName": "Bacon",
      "additionalPrice": 1.50,
      "displayOrder": 2
    },
    {
      "optionName": "Sausage",
      "additionalPrice": 1.50,
      "displayOrder": 3
    },
    {
      "optionName": "Vegetarian Patty",
      "additionalPrice": 1.00,
      "displayOrder": 4
    }
  ]
}

// Add dietary options
POST /api/v1/menu-items/20/customization-groups
{
  "groupName": "Dietary Options",
  "selectionType": "MultiSelect",
  "minimumSelections": 0,
  "maximumSelections": 3,
  "isRequired": false,
  "displayOrder": 3,
  "options": [
    {
      "optionName": "No Salt",
      "additionalPrice": 0,
      "displayOrder": 1
    },
    {
      "optionName": "Gluten Free",
      "additionalPrice": 0.75,
      "displayOrder": 2
    },
    {
      "optionName": "Vegan",
      "additionalPrice": 1.00,
      "displayOrder": 3
    },
    {
      "optionName": "Low Sugar",
      "additionalPrice": 0.50,
      "displayOrder": 4
    }
  ]
}

// ============================================
// HOW TO USE THIS FLEXIBLE SYSTEM:
//
// 1. Create a RestaurantConfig for your restaurant type
// 2. Create Categories specific to your cuisine
// 3. Create Subcategories for better organization
// 4. Create MenuItems with your base offerings
// 5. Add Variants for sizes, portions, or preparations
// 6. Add CustomizationGroups for toppings, sides, etc.
// 7. Create ComboDeal for bundled offerings
// 8. Set BundlePricing for quantity discounts
//
// The system handles:
// ✓ Multi-level categorization (Category -> Subcategory)
// ✓ Item variants (sizes, portions, preparations)
// ✓ Customization options (toppings, sides, modifications)
// ✓ Dietary restrictions (vegetarian, vegan, gluten-free)
// ✓ Spice levels (for Indian, Asian cuisines)
// ✓ Nutritional information
// ✓ Allergen information
// ✓ Combo deals
// ✓ Quantity-based bundle pricing
// ✓ Seasonal items
// ✓ Price discounts/sales
// ✓ Restaurant-specific configurations
// ============================================
