﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using UGoFor.API.DAL;
using RandomNameGeneratorLibrary;

namespace UGoFor.API.Models
{
    public class DepositAddr : IFromDataReader<DepositAddr>
    {

        public int? Id { get; set; }
        public string SubmittedAddr { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public string DepositAddress { get; set; }
        public int? Status { get; set; }
        public DateTime? StatusUpdate { get; set; }
        public string AmountToSend { get; set; }

        public DepositAddr FromDataReader(IDataReader dr)
        {
            DepositAddr depositAddr = new DepositAddr();
            depositAddr.Id = dr["Id"] is DBNull ? null : dr["Id"] as Int32?;
            depositAddr.SubmittedAddr = dr["SubmittedAddr"] is DBNull ? null : dr["SubmittedAddr"].ToString();
            depositAddr.SubmittedOn = dr["SubmittedOn"] is DBNull ? null : dr["SubmittedOn"] as DateTime?;
            depositAddr.DepositAddress = dr["DepositAddr"] is DBNull ? null : dr["DepositAddr"].ToString();
            depositAddr.Status = dr["Status"] is DBNull ? null : dr["Status"] as Int32?;
            depositAddr.StatusUpdate = dr["StatusUpdate"] is DBNull ? null : dr["StatusUpdate"] as DateTime?;
            depositAddr.AmountToSend = dr["AmountToSend"] is DBNull ? null : dr["AmountToSend"].ToString();
            return depositAddr;
        }

        private int DepositAddrExists(string depositAddr)
        {
            return new DepositAddrDAL().DepositAddrExists(depositAddr);
        }

        public string InsertDepositAddr(string[] submittedAddr)
        {
            DateTime submittedOn = DateTime.Now;
            DepositAddrDAL depoDAL = new DepositAddrDAL();
            string depositedAddr = GenerateRandomName();
            while (DepositAddrExists(depositedAddr) == 1)
            {
                depositedAddr = GenerateRandomName();
            }

            foreach (string sAddr in submittedAddr)
            {
                if(!string.IsNullOrEmpty(sAddr))
                    depoDAL.InsertDepositAddr(sAddr, depositedAddr, submittedOn);
            }

            return depositedAddr;
        }
        
        public List<DepositAddr> SelectAllDepositAddr(int status)
        {
            return new DepositAddrDAL().SelectAllDepositAddr(status);
        }

        private string GenerateRandomName()
        {
            return new PersonNameGenerator().GenerateRandomFirstAndLastName();            
        }

        public void UpdateDepositAddress(SimpleSend submitaddr)
        {
            new DepositAddrDAL().UpdateDepositAddress(submitaddr);
        }
    }

    public class SimpleSend
    {
        public string DepositAddress;
        public string Amount;
        public int Status;
    }
}
