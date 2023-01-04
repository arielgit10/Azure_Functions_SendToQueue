using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FunctionAppAriel;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppTimerTrigger
{
    public class TimerFunctionDemo
    {
        private readonly ILogger<TimerFunctionDemo> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly QueueClient _queueClient;
        private readonly IServerRepository _serverRepository;
        private const string _queueName = "cola1";
        private string _connectionString;
        private List<Account> _newAccounts;
        private List<Guid> _accountsId;


        public TimerFunctionDemo(ILogger<TimerFunctionDemo> logger, IMemoryCache memoryCache, IServerRepository serverRepository)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _serverRepository = serverRepository;
            _connectionString = Environment.GetEnvironmentVariable("QueueConnection", EnvironmentVariableTarget.Process);

            _queueClient = new QueueClient(_connectionString, _queueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });
            _newAccounts = new List<Account>();
        }

        [FunctionName("TimeTrigger")]
        public async Task RunAsync([TimerTrigger("%TimerSchedule%")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");


            var accounts = await ReadDataBase();

            if (accounts is not null)
            {
                SetAccountsIdFromCache(accounts);
            }
            if (_newAccounts.Any())
            {
                dynamic body = JsonConvert.SerializeObject(_newAccounts);
                if (_queueClient.Exists())
                {
                    _queueClient.SendMessage(body);
                }
                _newAccounts.Clear();
            }
        }
        private async Task<List<Account>> ReadDataBase()
        {
            var accounts = await _serverRepository.GetAccounts();
            return accounts;
        }
        public void SetAccountsIdFromCache(List<Account> listAccounts)
        {
            var accountsListCacheKey = "accountList";

            if (!_memoryCache.TryGetValue(accountsListCacheKey, out _accountsId))
            {
                _accountsId = new List<Guid>();
                _memoryCache.Set(accountsListCacheKey, _accountsId, TimeSpan.FromMinutes(15));
            }

            _newAccounts = listAccounts.Where(account => !_accountsId.Contains(account.AccountId)).ToList();

            foreach (var account in _newAccounts)
            {
                if (!_accountsId.Contains(account.AccountId))
                {
                    _accountsId.Add(account.AccountId);
                }
            }
        }
    }
}
