if (weatherModel.Temperature < -40)
        {
          throw new ValidationException("Temperature value is too low.");
        }