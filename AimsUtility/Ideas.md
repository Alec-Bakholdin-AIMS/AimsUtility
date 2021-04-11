# Ideas

- What to do about input errors?
  - The input has two types of errors: line and column errors
    - Perhaps there is a dictionary for each, or maybe I make another structure?
    - Dictionary idea:
      - One dictionary for line exceptions:
        - DuplicatePrimaryKeyException $\rightarrow$ If the user wants, this exception will be thrown when there are primary key clashes
        - MismatchedValueException $\rightarrow$ The more common case for this libary. When there are duplicate primary keys, only thrown if the values are different.
      - One dictionary for column exceptions:
        - InvalidFormatException $\rightarrow$ The data of the input table is formatted incorrectly
        - ValueNotFoundInApiException $\rightarrow$ No corresponding data was found in the API
    - Structure:
      - Contains a dictionary of each also?
      - Can generate error report from here, can also write it to Azure or whatever
      - This probably offers better modularity
      - This is the option I'm going for haha
    - So for each dictionary, it will be a list of obejcts
---

- How do we handle input errors in the output?
  - We are currently tracking input errors, which is what we need to report to the client
  - How do we know when the output row is invalid?
  - Solutions:
    - Keep track of which input rows map to which output rows
      - This is really difficult to manage for the person using the library
    - Have a dictionary which maps primary keys of the output row to a boolean (success/failure)
      - We can then have a function that determines if the row was successful in its completion
      - I think this one is actually good
      - We can also have a function where we remove all the invalid rows, and in that case, the tables for line information and siz information won't even be called, based on what I have planned for the Order object.