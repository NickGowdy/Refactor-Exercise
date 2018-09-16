Hey guys,

I tried to refactor your code to the best I can with the requirements you set me.

Because CustomerService is so tightly coupled with it's dependencies, I decided to inherit from it with a new service specifically for Customer and Companies because it gets the customer's company also. I broke down the customer service code into protected virtual helper methods. This enabled me to override them in my new service class and replace the tightly coupled dependenicies with my injected interfaces.

Finally I created a wrapper class for the static CustomerDataAccess, otherwise I wouldn't of been able to write tests for scenarios where the code returns true.

I hope you like my code and that you want to invite me to ASOS for a face-to-face interview.

Nick :D
