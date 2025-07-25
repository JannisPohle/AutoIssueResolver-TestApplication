  }

  private void Dispose(bool disposing)
  {
    if (disposing)
    {
      _httpClient.Dispose();
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  #endregion
}
