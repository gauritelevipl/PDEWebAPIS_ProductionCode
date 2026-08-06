using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;

namespace PDEWebAPIS.ContractRepo
{
    public interface ICommonRepository
    {
        Task<UserMaster?> GetUserByIdAsync(int? userId);
        Task<MutationGiverTakerDTL?> DuplicateOwnerCheck(BhadepattaGiverInputModel? bhadepattaGiverInputModel);

        Task<int> GetMutationCtsNoId(string? applicationId, string nabhu);

        Task<ApplicationDTL?> GetApplicationData(string? applicationId);

        Task<PropertyTypeMaster?> GetPropertyType();


        Task<string?> SaveAddress(string? imageSrc, string? imgName, BhadepattaModel bhadepattaModel, int BhadepattaNondID, string FileTypeFlag, string? folderpath);
        Task<string?> UpdateEntityValues<TEntity>(TEntity entity) where TEntity : class;

        Task<string?> AddEntityValues<TEntity>(TEntity entity) where TEntity : class;
        Task<string?> RemoveEntityValues<TEntity>(TEntity entity) where TEntity : class;
        Task<string?> RemoveUpdates<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync();

        Task<MutationGiverTakerDTL?> GetMutationDtlData(int? mutationDtlId);

        //Task<BhadepattaInfoDtl?> GetBhadepattaInfoData(string? applicationid);

        Task<bool> checkBhadepattaInfoDuplicateEntry(string? applicationid);
        Task<BhadepattaInfoDtl> GetBhadepattaInfoData(string? applicationid);


    }
}
