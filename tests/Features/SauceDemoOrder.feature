Feature: SauceDemo Order Flow

  Scenario: Add five products to cart and complete checkout with verification
    Given I navigate to SauceDemo login page "https://www.saucedemo.com/?utm_source=chatgpt.com"
    When I log in with username "standard_user" and password "secret_sauce"
    And I add the following 5 products to the cart and verify the button changes to "Remove":
      | ProductName                         |
      | Sauce Labs Backpack                 |
      | Sauce Labs Bike Light               |
      | Sauce Labs Bolt T-Shirt             |
      | Sauce Labs Fleece Jacket            |
      | Sauce Labs Onesie                   |
    And I click on the shopping cart
    And I click on the checkout button
    And I enter checkout information with first name "John", last name "Doe", and postal code "12345"
    And I click continue to proceed to checkout overview
    Then I verify the quantity and description for all added products:
      | ProductName                         | Quantity | Description                                                                                                                              |
      | Sauce Labs Backpack                 | 1        | carry.allTheThings() with the sleek, streamlined Sly Pack that melds uncompromising style with unequaled laptop and tablet protection.  |
      | Sauce Labs Bike Light               | 1        | A red light isn't the desired state in testing but it sure helps when riding your bike at night. Water-resistant with 3 lighting modes, 1 AAA battery included. |
      | Sauce Labs Bolt T-Shirt             | 1        | Get your testing superhero on with the Sauce Labs bolt T-shirt. From American Apparel, 100% ringspun combed cotton, heather gray with red bolt. |
      | Sauce Labs Fleece Jacket            | 1        | It's not every day that you come across a midweight quarter-zip fleece jacket capable of handling everything from a relaxing day outdoors to a busy day at the office. |
      | Sauce Labs Onesie                   | 1        | Rib snap infant onesie for the junior automation engineer in development. Reinforced 3-snap bottom closure, two-needle hemmed sleeved and bottom won't unravel. |
    When I click on the finish button
    And I click on the back home button
    Then I should be navigated back to the inventory home page
