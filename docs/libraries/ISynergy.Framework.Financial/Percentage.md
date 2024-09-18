# Percentage

The Percentage class in ISynergy.Framework.Financial namespace is being explained here.

This code defines a static class called Percentage that provides utility methods for performing percentage-based calculations. The class is designed to help with financial calculations involving percentages.

The class contains two main methods:

- CalculatePercentageAmountOfAmount: This method calculates the percentage increase or decrease between two amounts. It takes two inputs: previousAmount and actualAmount, both as decimal numbers. The output is a decimal representing the percentage change. The method works by comparing the two amounts and calculating the difference as a percentage of the previous amount. If the previous amount is zero, it handles this special case by returning either 0 (if both amounts are the same) or 1 (if they're different).

- CalculateAmountOfPercentage: This method calculates a specific percentage of a given amount. It takes two inputs: amount (the base value) and percentage (the percentage to calculate), both as decimal numbers. The output is a decimal representing the calculated amount. The method works by dividing the amount by 100 to get 1% of the amount, then multiplying this by the given percentage.

The purpose of this code is to simplify common percentage calculations that might be needed in financial applications. It provides a standardized way to perform these calculations, ensuring consistency across an application.

The logic in these methods is relatively straightforward. For CalculatePercentageAmountOfAmount, it first checks if the previous amount is non-zero to avoid division by zero errors. If it's non-zero, it calculates the percentage change. If the previous amount is zero, it handles this as a special case.

For CalculateAmountOfPercentage, the logic is a simple mathematical operation: it divides the amount by 100 to get 1%, then multiplies by the desired percentage.

These methods can be useful in various financial scenarios, such as calculating price changes, discounts, or investment returns. By providing these as utility methods, the code allows developers to perform these calculations easily and consistently throughout their application.