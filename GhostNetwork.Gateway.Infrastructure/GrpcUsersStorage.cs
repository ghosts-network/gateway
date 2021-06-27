// using System;
// using System.Threading.Tasks;
// using Domain;
// using GhostNetwork.Gateway.Users;
// using GhostNetwork.Profiles.Grpc;
// using Google.Protobuf.WellKnownTypes;
// using Grpc.Core;
//
// namespace GhostNetwork.Gateway.Infrastructure
// {
//     public class GrpcUsersStorage : IUsersStorage
//     {
//         private readonly Profiles.Grpc.Profiles.ProfilesClient profilesClient;
//
//         public GrpcUsersStorage(Profiles.Grpc.Profiles.ProfilesClient profilesClient)
//         {
//             this.profilesClient = profilesClient;
//         }
//
//         public async Task<User> GetByIdAsync(Guid id)
//         {
//             try
//             {
//                 var profile = await profilesClient.GetByIdAsync(new ByIdRequest
//                 {
//                     Id = id.ToString()
//                 });
//
//                 return new User(Guid.Parse(profile.Id), profile.FirstName, profile.LastName, profile.Gender, profile.DateOfBirth?.ToDateTime());
//             }
//             catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
//             {
//                 return null;
//             }
//         }
//
//         public async Task<DomainResult> UpdateAsync(User user)
//         {
//             var profile = await profilesClient.GetByIdAsync(new ByIdRequest
//             {
//                 Id = user.ToString()
//             });
//
//             profile.Gender = user.Gender;
//             profile.DateOfBirth = user.DateOfBirth.HasValue
//                 ? Timestamp.FromDateTime(user.DateOfBirth.Value)
//                 : null;
//
//             try
//             {
//                 await profilesClient.UpdateAsync(profile);
//                 return DomainResult.Success();
//             }
//             catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
//             {
//                 return DomainResult.Error("ERROR!!!!");
//             }
//         }
//     }
// }