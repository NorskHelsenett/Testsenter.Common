using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Shared.Common.Storage.Queue
{
    public enum QueueItemType
    {
        EbmsJob = 1,
        RequestReport = 2,
        ResponseReport = 3,
        RequestErrorReport = 4,
        StatusUpdate = 5,
        ReponseErrorReport = 6,
        RegresjonJob = 7,

        AutoTestRegister = 11,
        AutoTestStart = 8,
        AutoTestUpdate = 9,
        AutoTestStop = 10,
        AutoTestFinished = 12,

        HodorTriggerAllSurveillance = 13,
        HodorTriggerOneSurveillanceItem = 14,

        HodorIndexPreg = 15,
        HodorIndexHpr = 16,
        HodorIndexFlr = 17,
        HodorIndexBedReg = 18,
        HodorIndexAr = 19,
        HodorIndexResh = 20,
        HodorIndexHtk = 21,
        HodorIndexDifi = 22,
        HodorIndexOfr = 23,

        HodorIndexPregAll = 50,
        HodorIndexDifiAll = 51,


        //Hodor jobs
        HodorRemoveUnusedTags = 100,
        HodorRemoveDuplicatesHpr = 101,

        ForTesting = 2000,
        ForAsyncTesting = 3000,

        FLRJobConfirmGP = 10000,
        FLRJobGetGPPatientList = 10001,
        FLRJobGetGPContract = 10002,
        FLRJobGetGPContractForNav = 10003,
        FLRJobGetGPContractIdsOperatingInPostalCode = 10004,
        FLRJobGetGPContractsOnOffice = 10005,
        FLRJobGetGPWithAssociatedGPContracts = 10006,
        FLRJobGetPatientGPDetails = 10007,
        FLRJobGetPatientGPHistory = 10008,
        FLRJobGetPatientsGPDetails = 10009,
        FLRJobGetPatientsGPDetailsAtTime = 10010,
        FLRJobSearchForGP = 10011,

        Digitaldialogepjasync = 200001,
        Digitaldialogepjsync = 200002
    }
}
