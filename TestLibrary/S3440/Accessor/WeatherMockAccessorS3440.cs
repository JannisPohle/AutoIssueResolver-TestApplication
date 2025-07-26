if (!int.TryParse(argument, out var count))
      {
        count = 10; // Default count if parsing fails
      }