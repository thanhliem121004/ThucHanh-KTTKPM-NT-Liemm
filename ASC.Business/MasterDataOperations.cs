using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASC.Business.Interfaces;
using ASC.DataAccess.Interfaces;
using ASC.Model.Models;

namespace ASC.Business
{
    public class MasterDataOperations : IMasterDataOperations
    {
        private readonly IUnitOfWork _unitOfWork;

        public MasterDataOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MasterDataKey>> GetAllMasterKeysAsync()
        {
            var masterKeys = await _unitOfWork.Repository<MasterDataKey>().FindAllAsync();
            return masterKeys.ToList();
        }

        public async Task<List<MasterDataKey>> GetMasterKeyByNameAsync(string name)
        {
            var masterKeys = await _unitOfWork.Repository<MasterDataKey>().FindAllByPartitionKeyAsync(name);
            return masterKeys.ToList();
        }

        public async Task<bool> InsertMasterKeyAsync(MasterDataKey key)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Repository<MasterDataKey>().AddAsync(key);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }

        public async Task<bool> UpdateMasterKeyAsync(string originalPartitionKey, MasterDataKey key)
        {
            var masterKey = await _unitOfWork.Repository<MasterDataKey>().FindAsync(originalPartitionKey, key.RowKey);

            if (masterKey.IsActive == key.IsActive && masterKey.IsDeleted == key.IsDeleted)
            {
                masterKey.Name = key.Name;
                _unitOfWork.Repository<MasterDataKey>().Update(masterKey);
                _unitOfWork.CommitTransaction();
            }

            return true;
        }

        public async Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(string key)
        {
            try
            {
                var masterKeys = await _unitOfWork.Repository<MasterDataValue>().FindAllByPartitionKeyAsync(key);
                return masterKeys.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<List<MasterDataValue>> GetAllMasterValuesAsync()
        {
            var masterValues = await _unitOfWork.Repository<MasterDataValue>().FindAllAsync();
            return masterValues.ToList();
        }

        public async Task<MasterDataValue> GetMasterValueByNameAsync(string key, string name)
        {
            var masterValues = await _unitOfWork.Repository<MasterDataValue>().FindAsync(key, name);
            return masterValues;
        }

        public async Task<bool> InsertMasterValueAsync(MasterDataValue value)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Repository<MasterDataValue>().AddAsync(value);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }

        public async Task<bool> UpdateMasterValueAsync(string originalPartitionKey, string originalRowKey, MasterDataValue value)
        {
            var masterValue = await _unitOfWork.Repository<MasterDataValue>().FindAsync(originalPartitionKey, originalRowKey);

            masterValue.IsActive = value.IsActive;
            masterValue.IsDeleted = value.IsDeleted;
            masterValue.Name = value.Name;
            masterValue.Value = value.Value;

            _unitOfWork.Repository<MasterDataValue>().Update(masterValue);
            _unitOfWork.CommitTransaction();

            return true;
        }

        public async Task<bool> UploadBulkMasterData(List<MasterDataValue> values)
        {
            using (_unitOfWork)
            {
                foreach (var value in values)
                {
                    // Find if null insert MasterKey
                    var masterKey = await GetMasterKeyByNameAsync(value.PatititonKey);
                    if (!masterKey.Any())
                    {
                        await _unitOfWork.Repository<MasterDataKey>().AddAsync(new MasterDataKey()
                        {
                            Name = value.PatititonKey,
                            RowKey = Guid.NewGuid().ToString(),
                            PatititonKey = value.PatititonKey
                        });
                    }

                    // Find if null insert MasterValue
                    var masterValueList = await GetAllMasterValuesByKeyAsync(value.PatititonKey); // Đổi tên biến
                    var masterValuesByKey = masterValueList.FirstOrDefault(p => p.Name == value.Name);
                    if (masterValuesByKey == null)
                    {
                        await _unitOfWork.Repository<MasterDataValue>().AddAsync(value);
                    }
                    else
                    {
                        masterValuesByKey.IsActive = value.IsActive; // Sửa lỗi: gán cho masterValuesByKey
                        masterValuesByKey.IsDeleted = value.IsDeleted; // Sửa lỗi: gán cho masterValuesByKey
                        masterValuesByKey.Name = value.Name; // Sửa lỗi: gán cho masterValuesByKey
                        _unitOfWork.Repository<MasterDataValue>().Update(masterValuesByKey); // Sửa lỗi: update masterValuesByKey
                    }
                }

                _unitOfWork.CommitTransaction();
                return true;
            }
        }
    }
}
