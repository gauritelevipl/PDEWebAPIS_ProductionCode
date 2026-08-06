using Microsoft.EntityFrameworkCore;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.Data;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;

namespace PDEWebAPIS.ContractRepo
{
    public class CommonRepository : ICommonRepository
    {
        private readonly AppDBContext _context;
        private readonly AppDBContextR _contextR;
        private readonly ILogger<CommonRepository> _logger;
        public CommonRepository(AppDBContext context, AppDBContextR contextR, ILogger<CommonRepository> logger)
        {
            _context = context;
            _contextR = contextR;
            _logger = logger;
        }
        public async Task<UserMaster?> GetUserByIdAsync(int? userId)
        {
            try
            {
                if (userId != null)
                {
                    _logger.LogInformation("User id in GetUserByIdAsync: " + userId);
                    return await _contextR.userMasters.FirstOrDefaultAsync(u => u.userid == userId);
                }
                else
                {
                    _logger.LogInformation("User id is null in GetUserByIdAsync");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception occurs in GetUserByIdAsync" + ex.Message);
                return null;
            }


        }

        public async Task<MutationGiverTakerDTL?> DuplicateOwnerCheck(BhadepattaGiverInputModel? bhadepattaGiverInputModel)
        {
            try
            {
                if (bhadepattaGiverInputModel != null)
                {
                    _logger.LogInformation("DuplicateOwnerCheck method Data is not null");

                    return await _contextR.mutationDTL.Include(app => app.applicationDTL).Where(x => x.applicationDTL!.applicationid!.Equals(bhadepattaGiverInputModel.applicationid)
                    && x.isTaker == 0 && x.isDeleted == false &&
                    x.owner_number == bhadepattaGiverInputModel.ownerNo
                    && x.cts_number == bhadepattaGiverInputModel.ctsNo &&
                     x.mutation_srno == bhadepattaGiverInputModel.mutationSroNo &&
                     x.owner_village_code == bhadepattaGiverInputModel.village_code
                    ).FirstOrDefaultAsync()!;

                }
                else
                {
                    _logger.LogInformation("DuplicateOwnerCheck method Data is null");
                    return null;
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception occurs in DuplicateOwnerCheck" + ex.ToString());
                return null;
            }


            //return fetchedData;
        }


        public async Task<int> GetMutationCtsNoId(string? applicationId, string? nabhu)
        {
            try
            {
                if (applicationId != null && nabhu != null)
                {
                    _logger.LogInformation("nabhu and application id is not null in GetMutationCtsNoId");
                    return await _contextR.mutationCTSNoDTLs.Where(x => x.applicationDTL.applicationid == applicationId && x.selected_city_servey_no == nabhu).
                        Select(u => u.mutation_cts_no_id)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    _logger.LogInformation("nabhu and application id is null in GetMutationCtsNoId");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception occurs in GetMutationCtsNoId" + ex.Message);
                return 0;
            }
        }

        public async Task<ApplicationDTL?> GetApplicationData(string? applicationId)
        {
            try
            {
                if (applicationId != null)
                {
                    _logger.LogInformation("application id in GetApplicationData: " + applicationId);
                    return await _contextR.applicationDTL.Where(x => x.applicationid == applicationId && x.isDeleted == false).FirstOrDefaultAsync();
                }
                else
                {
                    _logger.LogInformation("application id is null in GetApplicationData");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception occurs in GetApplicationData" + ex.Message);
                return null;
            }
        }

        public async Task<PropertyTypeMaster?> GetPropertyType()
        {
            try
            {
                _logger.LogInformation("GetPropertyType data");
                return await _contextR.propertyTypes.FirstOrDefaultAsync(s => s.propertytypeid == 0)!;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception occurs in GetPropertyType" + ex.Message);
                return null;
            }
        }


        public async Task<string?> SaveAddress(string? imageSrc, string? imgName, BhadepattaModel bhadepattaModel, int BhadepattaNondID, string FileTypeFlag, string? folderpath)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            bool checkAddressFlag, checkSignFlag = true;
            string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
            string[] AddressData = imageSrc!.Split(",");

            string imageName = System.IO.Path.GetFileNameWithoutExtension(imgName!);
            if (methodForFile.ContainsSpecialCharacters(imageName))
            {
                return imgName! + " Image Name contains special Characters";
            }
            else
            {
                checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], imgName!, BhadepattaNondID.ToString(), FileTypeFlag, folderpath + @"\", CurrentDateTime);
                if (checkAddressFlag)
                {
                    return "Success";
                }
                else
                {
                    return "Error occur while saving image";
                }

            }
        }

        public async Task<string?> UpdateEntityValues<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {

                _context.Entry(entity).CurrentValues.SetValues(entity);
                //_context.Update(entity);
                _logger.LogInformation("UpdateEntityValues: Value updated" + entity);
                await _context.SaveChangesAsync();
                return "Update";
            }
            catch (Exception ex)
            {
                _logger.LogInformation("UpdateEntityValues: Error" + ex.ToString());
                return "Something went wrong";
            }
        }
        public async Task<string?> AddEntityValues<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                _logger.LogInformation("AddEntityValues: Value Added");
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
                var conn = _context.Database.GetDbConnection();
                _logger.LogInformation("[DB] Connected to:" + conn.DataSource + "," + "Database: " + conn.Database);
                _logger.LogInformation("AddEntityValues: {Count} rows saved", entity);

                return "Added";
            }
            catch (Exception ex)
            {
                _logger.LogInformation("AddEntityValues: Error" + ex.ToString());
                return "Something went wrong";
            }
        }
        public async Task<string?> RemoveEntityValues<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                _logger.LogInformation("RemoveEntityValues: Value Added");
                _context.Remove(entity);
                await _context.SaveChangesAsync();
                return "Remove";
            }
            catch (Exception ex)
            {
                _logger.LogInformation("RemoveEntityValues: Error" + ex.ToString());
                return "Something went wrong";

            }
        }
        public async Task<string?> RemoveUpdates<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                _logger.LogInformation("RemoveUpdates: Value Added");
                _context.Entry(entity).State = EntityState.Detached;
                await _context.SaveChangesAsync();
                return "RemoveUpdates";
            }
            catch (Exception ex)
            {
                _logger.LogInformation("RemoveUpdates: Error" + ex.ToString());
                return "Something went wrong";
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                _logger.LogInformation("SaveChangesAsync: Changes Saved");
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("SaveChangesAsync: Error while saving data" + ex.ToString());
                return 0;
            }
        }


        public async Task<MutationGiverTakerDTL?> GetMutationDtlData(int? mutationDtlId)
        {
            if (mutationDtlId != null)
            {
                return await _contextR.mutationDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).
                     Where(data => data.mutation_givertaker_id.Equals(mutationDtlId) && data.isDeleted == false).FirstOrDefaultAsync()!;
            }
            else
            {
                return null;
            }
        }

        //public async Task<BhadepattaInfoDtl?> GetBhadepattaInfoData(string? applicationid);


        public async Task<bool> checkBhadepattaInfoDuplicateEntry(string? applicationid)
        {
            if (applicationid != null)
            {
                return await _contextR.bhadepattaInfoDtl.AnyAsync(x => x.applicationid == applicationid && x.isDeleted == false);
            }
            else
            {
                return false;
            }
        }

        public async Task<BhadepattaInfoDtl> GetBhadepattaInfoData(string? applicationid)
        {
            if (applicationid != null)
            {
                return await _contextR.bhadepattaInfoDtl.Where(data => data.applicationid == applicationid && data.isDeleted == false).FirstOrDefaultAsync()!;
            }
            else
            {
                return null;
            }
        }
    }
}
