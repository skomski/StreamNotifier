﻿using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Helper.Extensions {
  public static class BackgroundWorkerExtensions {
    public static void RunWorker(this BackgroundWorker worker, object argument = null) {
      Contract.Requires(worker != null);

      if (!worker.IsBusy) {
        worker.RunWorkerAsync(argument);
      }
    }
  }
}