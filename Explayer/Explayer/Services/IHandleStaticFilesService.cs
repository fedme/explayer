﻿using Acr.UserDialogs;
using System.Threading.Tasks;

namespace Explayer.Services
{
    interface IHandleStaticFilesService
    {
        Task InitializeStaticFiles();
        string DirectoryPath { get; }
    }

    public abstract class AbstractHandleStaticFilesService : IHandleStaticFilesService
    {

        public async Task InitializeStaticFiles()
        {
            if (!DirectoryPopulated())
            {
                using (var loading = UserDialogs.Instance.Loading("Setting up for first time use"))
                {
                    loading.Show();
                    await LoadHtmlFromResource();
                    await Task.Delay(3000);
                }
            }
        }

        public abstract string DirectoryPath { get; }

        protected virtual bool DirectoryPopulated()
        {
            return System.IO.Directory.Exists(DirectoryPath);
        }

        protected abstract Task LoadHtmlFromResource();
    }
}