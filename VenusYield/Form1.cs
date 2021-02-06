using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Nethereum.Contracts;
using Nethereum.Web3;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace VenusYield
{
    public partial class Form1 : Form
    {
        public string BSCAddress { get; private set; }
        public string TelegramBot { get; private set; }
        public uint TelegramChatID { get; private set; }
        public uint BorrowUnder { get; private set; }
        public uint BorrowOver { get; private set; }
        public uint RefreshMins { get; private set; }

        readonly string VenusAPIvToken = "https://api.venus.io/api/vtoken";
        readonly string BinanceAPIticker = "https://api.binance.com/api/v3/ticker/24hr?symbol=";
        readonly string BSCendpoint = "https://bsc-dataseed1.binance.org";        

        readonly string BSCvXVScontract = "0x151b1e2635a717bcdc836ecd6fbb62b674fe3e1d";
        readonly string BSCvSXPcontract = "0x2fF3d0F6990a40261c66E1ff2017aCBc282EB6d0";
        readonly string BSCvUSDCcontract = "0xecA88125a5ADbe82614ffC12D0DB554E2e2867C8";
        readonly string BSCvUSDTcontract = "0xfD5840Cd36d94D7229439859C0112a4185BC0255";
        readonly string BSCvBUSDcontract = "0x95c78222B3D6e262426483D42CfA53685A67Ab9D";
        readonly string BSCvBNBcontract = "0xA07c5b74C9B40447a954e1466938b865b6BBea36";
        readonly string BSCvBTCcontract = "0x882C173bC7Ff3b7786CA16dfeD3DFFfb9Ee7847B";
        readonly string BSCvETHcontract = "0xf508fCD89b8bd15579dc79A6827cB4686A3592c8";
        readonly string BSCvLTCcontract = "0x57A5297F2cB2c0AaC9D554660acd6D385Ab50c6B";
        readonly string BSCvXRPcontract = "0xB248a295732e0225acd3337607cc01068e3b9c10";
        readonly string BSCvBCHcontract = "0x5F0388EBc2B94FA8E123F404b79cCF5f40b29176";
        readonly string BSCvDOTcontract = "0x1610bc33319e9398de5f57b33a5b184c806ad217";
        readonly string BSCvLINKcontract = "0x650b940a1033B8A1b1873f78730FcFC73ec11f1f";
        readonly string BSCvBETHcontract = "0x972207A639CC1B374B893cc33Fa251b55CEB7c07";
        readonly string BSCvDAIcontract = "0x334b3eCB4DCa3593BCCC3c7EBD1A1C1d1780FBF1";
        readonly string BSCvFILcontract = "0xf91d58b5aE142DAcC749f58A49FCBac340Cb0343";
        readonly string BSCVAIcontract = "0x4bd17003473389a42daf6a0a729f6fdb328bbbd7";
        readonly string BSCVAIvaultcontract = "0x0667Eed0a0aAb930af74a3dfeDD263A73994f216";
        
        string ABIvTokens = @"[{'inputs':[{'internalType':'address','name':'underlying_','type':'address'},{'internalType':'contract ComptrollerInterface','name':'comptroller_','type':'address'},{'internalType':'contract InterestRateModel','name':'interestRateModel_','type':'address'},{'internalType':'uint256','name':'initialExchangeRateMantissa_','type':'uint256'},{'internalType':'string','name':'name_','type':'string'},{'internalType':'string','name':'symbol_','type':'string'},{'internalType':'uint8','name':'decimals_','type':'uint8'},{'internalType':'address payable','name':'admin_','type':'address'},{'internalType':'address','name':'implementation_','type':'address'},{'internalType':'bytes','name':'becomeImplementationData','type':'bytes'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'uint256','name':'cashPrior','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'interestAccumulated','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'borrowIndex','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'totalBorrows','type':'uint256'}],'name':'AccrueInterest','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'internalType':'address','name':'owner','type':'address'},{'indexed':true,'internalType':'address','name':'spender','type':'address'},{'indexed':false,'internalType':'uint256','name':'amount','type':'uint256'}],'name':'Approval','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'borrower','type':'address'},{'indexed':false,'internalType':'uint256','name':'borrowAmount','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'accountBorrows','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'totalBorrows','type':'uint256'}],'name':'Borrow','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'uint256','name':'error','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'info','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'detail','type':'uint256'}],'name':'Failure','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'liquidator','type':'address'},{'indexed':false,'internalType':'address','name':'borrower','type':'address'},{'indexed':false,'internalType':'uint256','name':'repayAmount','type':'uint256'},{'indexed':false,'internalType':'address','name':'vTokenCollateral','type':'address'},{'indexed':false,'internalType':'uint256','name':'seizeTokens','type':'uint256'}],'name':'LiquidateBorrow','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'minter','type':'address'},{'indexed':false,'internalType':'uint256','name':'mintAmount','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'mintTokens','type':'uint256'}],'name':'Mint','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'oldAdmin','type':'address'},{'indexed':false,'internalType':'address','name':'newAdmin','type':'address'}],'name':'NewAdmin','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'contract ComptrollerInterface','name':'oldComptroller','type':'address'},{'indexed':false,'internalType':'contract ComptrollerInterface','name':'newComptroller','type':'address'}],'name':'NewComptroller','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'oldImplementation','type':'address'},{'indexed':false,'internalType':'address','name':'newImplementation','type':'address'}],'name':'NewImplementation','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'contract InterestRateModel','name':'oldInterestRateModel','type':'address'},{'indexed':false,'internalType':'contract InterestRateModel','name':'newInterestRateModel','type':'address'}],'name':'NewMarketInterestRateModel','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'oldPendingAdmin','type':'address'},{'indexed':false,'internalType':'address','name':'newPendingAdmin','type':'address'}],'name':'NewPendingAdmin','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'uint256','name':'oldReserveFactorMantissa','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'newReserveFactorMantissa','type':'uint256'}],'name':'NewReserveFactor','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'redeemer','type':'address'},{'indexed':false,'internalType':'uint256','name':'redeemAmount','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'redeemTokens','type':'uint256'}],'name':'Redeem','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'payer','type':'address'},{'indexed':false,'internalType':'address','name':'borrower','type':'address'},{'indexed':false,'internalType':'uint256','name':'repayAmount','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'accountBorrows','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'totalBorrows','type':'uint256'}],'name':'RepayBorrow','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'benefactor','type':'address'},{'indexed':false,'internalType':'uint256','name':'addAmount','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'newTotalReserves','type':'uint256'}],'name':'ReservesAdded','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'internalType':'address','name':'admin','type':'address'},{'indexed':false,'internalType':'uint256','name':'reduceAmount','type':'uint256'},{'indexed':false,'internalType':'uint256','name':'newTotalReserves','type':'uint256'}],'name':'ReservesReduced','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'internalType':'address','name':'from','type':'address'},{'indexed':true,'internalType':'address','name':'to','type':'address'},{'indexed':false,'internalType':'uint256','name':'amount','type':'uint256'}],'name':'Transfer','type':'event'},{'payable':true,'stateMutability':'payable','type':'fallback'},{'constant':false,'inputs':[],'name':'_acceptAdmin','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'addAmount','type':'uint256'}],'name':'_addReserves','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'reduceAmount','type':'uint256'}],'name':'_reduceReserves','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'contract ComptrollerInterface','name':'newComptroller','type':'address'}],'name':'_setComptroller','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'implementation_','type':'address'},{'internalType':'bool','name':'allowResign','type':'bool'},{'internalType':'bytes','name':'becomeImplementationData','type':'bytes'}],'name':'_setImplementation','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'contract InterestRateModel','name':'newInterestRateModel','type':'address'}],'name':'_setInterestRateModel','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address payable','name':'newPendingAdmin','type':'address'}],'name':'_setPendingAdmin','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'newReserveFactorMantissa','type':'uint256'}],'name':'_setReserveFactor','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'accrualBlockNumber','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'accrueInterest','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'admin','outputs':[{'internalType':'address payable','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'owner','type':'address'},{'internalType':'address','name':'spender','type':'address'}],'name':'allowance','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'spender','type':'address'},{'internalType':'uint256','name':'amount','type':'uint256'}],'name':'approve','outputs':[{'internalType':'bool','name':'','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'owner','type':'address'}],'name':'balanceOf','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'owner','type':'address'}],'name':'balanceOfUnderlying','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'borrowAmount','type':'uint256'}],'name':'borrow','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'account','type':'address'}],'name':'borrowBalanceCurrent','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'account','type':'address'}],'name':'borrowBalanceStored','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'borrowIndex','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'borrowRatePerBlock','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'comptroller','outputs':[{'internalType':'contract ComptrollerInterface','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'internalType':'uint8','name':'','type':'uint8'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'bytes','name':'data','type':'bytes'}],'name':'delegateToImplementation','outputs':[{'internalType':'bytes','name':'','type':'bytes'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'internalType':'bytes','name':'data','type':'bytes'}],'name':'delegateToViewImplementation','outputs':[{'internalType':'bytes','name':'','type':'bytes'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'exchangeRateCurrent','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'exchangeRateStored','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'account','type':'address'}],'name':'getAccountSnapshot','outputs':[{'internalType':'uint256','name':'','type':'uint256'},{'internalType':'uint256','name':'','type':'uint256'},{'internalType':'uint256','name':'','type':'uint256'},{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'getCash','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'implementation','outputs':[{'internalType':'address','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'interestRateModel','outputs':[{'internalType':'contract InterestRateModel','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'isVToken','outputs':[{'internalType':'bool','name':'','type':'bool'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'borrower','type':'address'},{'internalType':'uint256','name':'repayAmount','type':'uint256'},{'internalType':'contract VTokenInterface','name':'vTokenCollateral','type':'address'}],'name':'liquidateBorrow','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'mintAmount','type':'uint256'}],'name':'mint','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'name','outputs':[{'internalType':'string','name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'pendingAdmin','outputs':[{'internalType':'address payable','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'redeemTokens','type':'uint256'}],'name':'redeem','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'redeemAmount','type':'uint256'}],'name':'redeemUnderlying','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'repayAmount','type':'uint256'}],'name':'repayBorrow','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'borrower','type':'address'},{'internalType':'uint256','name':'repayAmount','type':'uint256'}],'name':'repayBorrowBehalf','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'reserveFactorMantissa','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'liquidator','type':'address'},{'internalType':'address','name':'borrower','type':'address'},{'internalType':'uint256','name':'seizeTokens','type':'uint256'}],'name':'seize','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'supplyRatePerBlock','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'internalType':'string','name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'totalBorrows','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'totalBorrowsCurrent','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'totalReserves','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'totalSupply','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'dst','type':'address'},{'internalType':'uint256','name':'amount','type':'uint256'}],'name':'transfer','outputs':[{'internalType':'bool','name':'','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'src','type':'address'},{'internalType':'address','name':'dst','type':'address'},{'internalType':'uint256','name':'amount','type':'uint256'}],'name':'transferFrom','outputs':[{'internalType':'bool','name':'','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'underlying','outputs':[{'internalType':'address','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'}]";
        string ABIVAItoken = @"[{'inputs':[{'internalType':'uint256','name':'chainId_','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':true,'internalType':'address','name':'src','type':'address'},{'indexed':true,'internalType':'address','name':'guy','type':'address'},{'indexed':false,'internalType':'uint256','name':'wad','type':'uint256'}],'name':'Approval','type':'event'},{'anonymous':true,'inputs':[{'indexed':true,'internalType':'bytes4','name':'sig','type':'bytes4'},{'indexed':true,'internalType':'address','name':'usr','type':'address'},{'indexed':true,'internalType':'bytes32','name':'arg1','type':'bytes32'},{'indexed':true,'internalType':'bytes32','name':'arg2','type':'bytes32'},{'indexed':false,'internalType':'bytes','name':'data','type':'bytes'}],'name':'LogNote','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'internalType':'address','name':'src','type':'address'},{'indexed':true,'internalType':'address','name':'dst','type':'address'},{'indexed':false,'internalType':'uint256','name':'wad','type':'uint256'}],'name':'Transfer','type':'event'},{'constant':true,'inputs':[],'name':'DOMAIN_SEPARATOR','outputs':[{'internalType':'bytes32','name':'','type':'bytes32'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'PERMIT_TYPEHASH','outputs':[{'internalType':'bytes32','name':'','type':'bytes32'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'','type':'address'},{'internalType':'address','name':'','type':'address'}],'name':'allowance','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'usr','type':'address'},{'internalType':'uint256','name':'wad','type':'uint256'}],'name':'approve','outputs':[{'internalType':'bool','name':'','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'','type':'address'}],'name':'balanceOf','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'usr','type':'address'},{'internalType':'uint256','name':'wad','type':'uint256'}],'name':'burn','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'internalType':'uint8','name':'','type':'uint8'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'guy','type':'address'}],'name':'deny','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'usr','type':'address'},{'internalType':'uint256','name':'wad','type':'uint256'}],'name':'mint','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'src','type':'address'},{'internalType':'address','name':'dst','type':'address'},{'internalType':'uint256','name':'wad','type':'uint256'}],'name':'move','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'name','outputs':[{'internalType':'string','name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'','type':'address'}],'name':'nonces','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'holder','type':'address'},{'internalType':'address','name':'spender','type':'address'},{'internalType':'uint256','name':'nonce','type':'uint256'},{'internalType':'uint256','name':'expiry','type':'uint256'},{'internalType':'bool','name':'allowed','type':'bool'},{'internalType':'uint8','name':'v','type':'uint8'},{'internalType':'bytes32','name':'r','type':'bytes32'},{'internalType':'bytes32','name':'s','type':'bytes32'}],'name':'permit','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'usr','type':'address'},{'internalType':'uint256','name':'wad','type':'uint256'}],'name':'pull','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'usr','type':'address'},{'internalType':'uint256','name':'wad','type':'uint256'}],'name':'push','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'guy','type':'address'}],'name':'rely','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'internalType':'string','name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'totalSupply','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'dst','type':'address'},{'internalType':'uint256','name':'wad','type':'uint256'}],'name':'transfer','outputs':[{'internalType':'bool','name':'','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'src','type':'address'},{'internalType':'address','name':'dst','type':'address'},{'internalType':'uint256','name':'wad','type':'uint256'}],'name':'transferFrom','outputs':[{'internalType':'bool','name':'','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'version','outputs':[{'internalType':'string','name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'','type':'address'}],'name':'wards','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'}]";
        string ABIVAIvault = @"[{'inputs':[],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':true,'internalType':'address','name':'oldAdmin','type':'address'},{'indexed':true,'internalType':'address','name':'newAdmin','type':'address'}],'name':'AdminTransfered','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'internalType':'address','name':'user','type':'address'},{'indexed':false,'internalType':'uint256','name':'amount','type':'uint256'}],'name':'Deposit','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'internalType':'address','name':'user','type':'address'},{'indexed':false,'internalType':'uint256','name':'amount','type':'uint256'}],'name':'Withdraw','type':'event'},{'constant':false,'inputs':[{'internalType':'contract VAIVaultProxy','name':'vaiVaultProxy','type':'address'}],'name':'_become','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'accXVSPerShare','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'admin','outputs':[{'internalType':'address','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'burnAdmin','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[],'name':'claim','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'_amount','type':'uint256'}],'name':'deposit','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'getAdmin','outputs':[{'internalType':'address','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'pendingAdmin','outputs':[{'internalType':'address','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'pendingRewards','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'pendingVAIVaultImplementation','outputs':[{'internalType':'address','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'_user','type':'address'}],'name':'pendingXVS','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'newAdmin','type':'address'}],'name':'setNewAdmin','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'internalType':'address','name':'_xvs','type':'address'},{'internalType':'address','name':'_vai','type':'address'}],'name':'setVenusInfo','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[],'name':'updatePendingRewards','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'internalType':'address','name':'','type':'address'}],'name':'userInfo','outputs':[{'internalType':'uint256','name':'amount','type':'uint256'},{'internalType':'uint256','name':'rewardDebt','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'vai','outputs':[{'internalType':'contract IBEP20','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'vaiVaultImplementation','outputs':[{'internalType':'address','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'internalType':'uint256','name':'_amount','type':'uint256'}],'name':'withdraw','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'xvs','outputs':[{'internalType':'contract IBEP20','name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'xvsBalance','outputs':[{'internalType':'uint256','name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'}]";

        List<string> StableCoins = new List<string> { "VAI", "USDT", "USDC", "DAI", "BUSD" };

        public Form1()
        {
            InitializeComponent();
            ReadSettings();
            bgwVenusYield.RunWorkerAsync();
        }

        public void ReadSettings()
        {
            var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            try
            {
                using (StreamReader file = File.OpenText(systemPath + @"\VenusYield.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Settings mySettings = (Settings)serializer.Deserialize(file, typeof(Settings));

                    BSCAddress = mySettings.BSCaddress;
                    TelegramBot = mySettings.TelegramBot;
                    TelegramChatID = mySettings.TelegramChatID;
                    BorrowUnder = mySettings.BorrowUnder;
                    BorrowOver = mySettings.BorrowOver;
                    RefreshMins = mySettings.RefreshRate;
                }                
            }
            catch { }
        }

        public async Task<vTokenBalances> tokenBalances(string Symbol, string BSCcontract, VenusAPIvTokenService vTokenService)
        {
            Web3 web3 = new Web3(BSCendpoint);
            vTokenBalances tokenBalances = new vTokenBalances();
            BinancePriceChange binancePriceChange = new BinancePriceChange();
            WebClient webclient = new WebClient();
            double Price = 0;
            if (Symbol == "BETH") Symbol = "ETH";

            try
            {
                if (StableCoins.Contains(Symbol))
                    Price = double.Parse(vTokenService.Data.Markets[vTokenService.Data.Markets.FindIndex(j => j.Symbol.Equals("v" + Symbol))].TokenPrice, CultureInfo.InvariantCulture);
                else
                {
                    binancePriceChange = BinancePriceChange.FromJson(webclient.DownloadString(BinanceAPIticker + Symbol + "USDT"));
                    Price = double.Parse(binancePriceChange.LastPrice, CultureInfo.InvariantCulture);
                }
                Contract vContract = web3.Eth.GetContract(ABIvTokens, BSCcontract);
                Function vSupplyFunction = vContract.GetFunction("balanceOf");
                dynamic vSupplyResult = await vSupplyFunction.CallAsync<dynamic>(BSCAddress);
                Function vBorrowFunction = vContract.GetFunction("borrowBalanceStored");
                dynamic vBorrowResult = await vBorrowFunction.CallAsync<dynamic>(BSCAddress);
                Function vExchangeRateFunction = vContract.GetFunction("exchangeRateStored");
                dynamic vExchangeRateResult = await vExchangeRateFunction.CallAsync<dynamic>();
                tokenBalances.Supply = (double)vSupplyResult * (double)vExchangeRateResult / 1E+36;
                tokenBalances.Borrow = (double)vBorrowResult / 1E+18;
                tokenBalances.PriceUSD = Price;
                tokenBalances.SupplyUSD = tokenBalances.PriceUSD * tokenBalances.Supply;
                tokenBalances.BorrowUSD = tokenBalances.PriceUSD * tokenBalances.Borrow;
            }
            catch { }

            return await Task.FromResult<vTokenBalances>(tokenBalances);
        }

        private async void bgwVenusYield_DoWorkAsync(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            vTokenBalances mytokenBalances = new vTokenBalances();
            VenusAPIvTokenService vTokenService = new VenusAPIvTokenService();
            WebClient webclient = new WebClient();
            Web3 web3 = new Web3(BSCendpoint);
            var TelegramMSG = "https://api.telegram.org/bot" + TelegramBot + "/sendMessage?chat_id=" + TelegramChatID.ToString() + "&parse_mode=HTML&text=";

            while (true)
            {
                try
                {
                    double TotalSupplyUSD = 0.0;
                    double TotalBorrowUSD = 0.0;
                    vTokenService = VenusAPIvTokenService.FromJson(webclient.DownloadString(VenusAPIvToken));

                    mytokenBalances = await tokenBalances("XVS", BSCvXVScontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceXVS.InvokeRequired)
                        lPriceXVS.Invoke(new MethodInvoker(delegate { lPriceXVS.Text = "$" + mytokenBalances.PriceUSD.ToString("N2", CultureInfo.InvariantCulture); }));
                    if (lSupplyXVS.InvokeRequired)
                        lSupplyXVS.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyXVS.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " XVS"; else lSupplyXVS.Text = "---"; }));
                    if (lBorrowXVS.InvokeRequired)
                        lBorrowXVS.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowXVS.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " XVS"; else lBorrowXVS.Text = "---"; }));

                    mytokenBalances = await tokenBalances("BTC", BSCvBTCcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceBTC.InvokeRequired)
                        lPriceBTC.Invoke(new MethodInvoker(delegate { lPriceBTC.Text = "$" + mytokenBalances.PriceUSD.ToString("N0", CultureInfo.InvariantCulture); }));
                    if (lSupplyBTC.InvokeRequired)
                        lSupplyBTC.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyBTC.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " BTC"; else lSupplyBTC.Text = "---"; }));
                    if (lBorrowBTC.InvokeRequired)
                        lBorrowBTC.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowBTC.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " BTC"; else lBorrowBTC.Text = "---"; }));

                    mytokenBalances = await tokenBalances("ETH", BSCvETHcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceETH.InvokeRequired)
                        lPriceETH.Invoke(new MethodInvoker(delegate { lPriceETH.Text = "$" + mytokenBalances.PriceUSD.ToString("N0", CultureInfo.InvariantCulture); }));
                    if (lSupplyETH.InvokeRequired)
                        lSupplyETH.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyETH.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " ETH"; else lSupplyETH.Text = "---"; }));
                    if (lBorrowETH.InvokeRequired)
                        lBorrowETH.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowETH.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " ETH"; else lBorrowETH.Text = "---"; }));

                    mytokenBalances = await tokenBalances("DOT", BSCvDOTcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceDOT.InvokeRequired)
                        lPriceDOT.Invoke(new MethodInvoker(delegate { lPriceDOT.Text = "$" + mytokenBalances.PriceUSD.ToString("N2", CultureInfo.InvariantCulture); }));
                    if (lSupplyDOT.InvokeRequired)
                        lSupplyDOT.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyDOT.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " DOT"; else lSupplyDOT.Text = "---"; }));
                    if (lBorrowDOT.InvokeRequired)
                        lBorrowDOT.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowDOT.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " DOT"; else lBorrowDOT.Text = "---"; }));

                    mytokenBalances = await tokenBalances("XRP", BSCvXRPcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceXRP.InvokeRequired)
                        lPriceXRP.Invoke(new MethodInvoker(delegate { lPriceXRP.Text = "$" + mytokenBalances.PriceUSD.ToString("N3", CultureInfo.InvariantCulture); }));
                    if (lSupplyXRP.InvokeRequired)
                        lSupplyXRP.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyXRP.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " XRP"; else lSupplyXRP.Text = "---"; }));
                    if (lBorrowXRP.InvokeRequired)
                        lBorrowXRP.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowXRP.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " XRP"; else lBorrowXRP.Text = "---"; }));

                    mytokenBalances = await tokenBalances("LTC", BSCvLTCcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceLTC.InvokeRequired)
                        lPriceLTC.Invoke(new MethodInvoker(delegate { lPriceLTC.Text = "$" + mytokenBalances.PriceUSD.ToString("N1", CultureInfo.InvariantCulture); }));
                    if (lSupplyLTC.InvokeRequired)
                        lSupplyLTC.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyLTC.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " LTC"; else lSupplyLTC.Text = "---"; }));
                    if (lBorrowLTC.InvokeRequired)
                        lBorrowLTC.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowLTC.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " LTC"; else lBorrowLTC.Text = "---"; }));

                    mytokenBalances = await tokenBalances("LINK", BSCvLINKcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceLINK.InvokeRequired)
                        lPriceLINK.Invoke(new MethodInvoker(delegate { lPriceLINK.Text = "$" + mytokenBalances.PriceUSD.ToString("N2", CultureInfo.InvariantCulture); }));
                    if (lSupplyLINK.InvokeRequired)
                        lSupplyLINK.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyLINK.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " LINK"; else lSupplyLINK.Text = "---"; }));
                    if (lBorrowLINK.InvokeRequired)
                        lBorrowLINK.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowLINK.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " LINK"; else lBorrowLINK.Text = "---"; }));

                    mytokenBalances = await tokenBalances("BCH", BSCvBCHcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceBCH.InvokeRequired)
                        lPriceBCH.Invoke(new MethodInvoker(delegate { lPriceBCH.Text = "$" + mytokenBalances.PriceUSD.ToString("N1", CultureInfo.InvariantCulture); }));
                    if (lSupplyBCH.InvokeRequired)
                        lSupplyBCH.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyBCH.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " BCH"; else lSupplyBCH.Text = "---"; }));
                    if (lBorrowBCH.InvokeRequired)
                        lBorrowBCH.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowBCH.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " BCH"; else lBorrowBCH.Text = "---"; }));

                    mytokenBalances = await tokenBalances("BNB", BSCvBNBcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceBNB.InvokeRequired)
                        lPriceBNB.Invoke(new MethodInvoker(delegate { lPriceBNB.Text = "$" + mytokenBalances.PriceUSD.ToString("N2", CultureInfo.InvariantCulture); }));
                    if (lSupplyBNB.InvokeRequired)
                        lSupplyBNB.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyBNB.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " BNB"; else lSupplyBNB.Text = "---"; }));
                    if (lBorrowBNB.InvokeRequired)
                        lBorrowBNB.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowBNB.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " BNB"; else lBorrowBNB.Text = "---"; }));

                    mytokenBalances = await tokenBalances("FIL", BSCvFILcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceFIL.InvokeRequired)
                        lPriceFIL.Invoke(new MethodInvoker(delegate { lPriceFIL.Text = "$" + mytokenBalances.PriceUSD.ToString("N2", CultureInfo.InvariantCulture); }));
                    if (lSupplyFIL.InvokeRequired)
                        lSupplyFIL.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyFIL.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " FIL"; else lSupplyFIL.Text = "---"; }));
                    if (lBorrowFIL.InvokeRequired)
                        lBorrowFIL.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowFIL.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " FIL"; else lBorrowFIL.Text = "---"; }));

                    mytokenBalances = await tokenBalances("SXP", BSCvSXPcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceSXP.InvokeRequired)
                        lPriceSXP.Invoke(new MethodInvoker(delegate { lPriceSXP.Text = "$" + mytokenBalances.PriceUSD.ToString("N3", CultureInfo.InvariantCulture); }));
                    if (lSupplySXP.InvokeRequired)
                        lSupplySXP.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplySXP.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " SXP"; else lSupplySXP.Text = "---"; }));
                    if (lBorrowSXP.InvokeRequired)
                        lBorrowSXP.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowSXP.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " SXP"; else lBorrowSXP.Text = "---"; }));

                    mytokenBalances = await tokenBalances("BETH", BSCvBETHcontract, null);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceBETH.InvokeRequired)
                        lPriceBETH.Invoke(new MethodInvoker(delegate { lPriceBETH.Text = "$" + mytokenBalances.PriceUSD.ToString("N0", CultureInfo.InvariantCulture); }));
                    if (lSupplyBETH.InvokeRequired)
                        lSupplyBETH.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyBETH.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " BETH"; else lSupplyBETH.Text = "---"; }));
                    if (lBorrowBETH.InvokeRequired)
                        lBorrowBETH.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowBETH.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " BETH"; else lBorrowBETH.Text = "---"; }));

                    mytokenBalances = await tokenBalances("USDT", BSCvUSDTcontract, vTokenService);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceUSDT.InvokeRequired)
                        lPriceUSDT.Invoke(new MethodInvoker(delegate { lPriceUSDT.Text = "$" + mytokenBalances.PriceUSD.ToString("N3", CultureInfo.InvariantCulture); }));
                    if (lSupplyUSDT.InvokeRequired)
                        lSupplyUSDT.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyUSDT.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " USDT"; else lSupplyUSDT.Text = "---"; }));
                    if (lBorrowUSDT.InvokeRequired)
                        lBorrowUSDT.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowUSDT.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " USDT"; else lBorrowUSDT.Text = "---"; }));

                    mytokenBalances = await tokenBalances("USDC", BSCvUSDCcontract, vTokenService);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceUSDC.InvokeRequired)
                        lPriceUSDC.Invoke(new MethodInvoker(delegate { lPriceUSDC.Text = "$" + mytokenBalances.PriceUSD.ToString("N3", CultureInfo.InvariantCulture); }));
                    if (lSupplyUSDC.InvokeRequired)
                        lSupplyUSDC.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyUSDC.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " USDC"; else lSupplyUSDC.Text = "---"; }));
                    if (lBorrowUSDC.InvokeRequired)
                        lBorrowUSDC.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowUSDC.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " USDC"; else lBorrowUSDC.Text = "---"; }));

                    mytokenBalances = await tokenBalances("DAI", BSCvDAIcontract, vTokenService);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceDAI.InvokeRequired)
                        lPriceDAI.Invoke(new MethodInvoker(delegate { lPriceDAI.Text = "$" + mytokenBalances.PriceUSD.ToString("N3", CultureInfo.InvariantCulture); }));
                    if (lSupplyDAI.InvokeRequired)
                        lSupplyDAI.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyDAI.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " DAI"; else lSupplyDAI.Text = "---"; }));
                    if (lBorrowDAI.InvokeRequired)
                        lBorrowDAI.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowDAI.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " DAI"; else lBorrowDAI.Text = "---"; }));

                    mytokenBalances = await tokenBalances("BUSD", BSCvBUSDcontract, vTokenService);
                    TotalSupplyUSD = TotalSupplyUSD + mytokenBalances.SupplyUSD; TotalBorrowUSD = TotalBorrowUSD + mytokenBalances.BorrowUSD;
                    if (lPriceBUSD.InvokeRequired)
                        lPriceBUSD.Invoke(new MethodInvoker(delegate { lPriceBUSD.Text = "$" + mytokenBalances.PriceUSD.ToString("N3", CultureInfo.InvariantCulture); }));
                    if (lSupplyBUSD.InvokeRequired)
                        lSupplyBUSD.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Supply != 0) lSupplyBUSD.Text = mytokenBalances.Supply.ToString("N2", CultureInfo.InvariantCulture) + " BUSD"; else lSupplyBUSD.Text = "---"; }));
                    if (lBorrowBUSD.InvokeRequired)
                        lBorrowBUSD.Invoke(new MethodInvoker(delegate { if (mytokenBalances.Borrow != 0) lBorrowBUSD.Text = mytokenBalances.Borrow.ToString("N2", CultureInfo.InvariantCulture) + " BUSD"; else lBorrowBUSD.Text = "---"; }));
                    //VAI   
                    double PriceVAI = 1.0;
                    Contract VAIContract = web3.Eth.GetContract(ABIVAItoken, BSCVAIcontract);
                    Function VAIBorrowFunction = VAIContract.GetFunction("balanceOf");
                    dynamic VAIBorrowResult = await VAIBorrowFunction.CallAsync<dynamic>(BSCAddress);
                    double BorrowVAI = (double)VAIBorrowResult / 1E+18;
                    if (lPriceVAI.InvokeRequired)
                        lPriceVAI.Invoke(new MethodInvoker(delegate { lPriceVAI.Text = "$" + PriceVAI.ToString("N0", CultureInfo.InvariantCulture); }));
                    if (lBorrowVAI.InvokeRequired)
                        lBorrowVAI.Invoke(new MethodInvoker(delegate { if (BorrowVAI != 0) lBorrowVAI.Text = BorrowVAI.ToString("N2", CultureInfo.InvariantCulture) + " VAI"; else lBorrowVAI.Text = "---"; }));
                    //VAIvault
                    Contract VAIvaultContract = web3.Eth.GetContract(ABIVAIvault, BSCVAIvaultcontract);
                    Function VAIvaultFunction = VAIvaultContract.GetFunction("userInfo");
                    dynamic VAIvaultResult = await VAIvaultFunction.CallAsync<dynamic>(BSCAddress);
                    double VAIvault = (double)VAIvaultResult / 1E+18;
                    if (lVAIvault.InvokeRequired)
                        lVAIvault.Invoke(new MethodInvoker(delegate { lVAIvault.Text = "VAI vault: " + "$" + VAIvault.ToString("N0", CultureInfo.InvariantCulture); }));
                    //Balance
                    if (lBalance.InvokeRequired)
                        lBalance.Invoke(new MethodInvoker(delegate { lBalance.Text = "Balance: " + "$" + TotalSupplyUSD.ToString("N0", CultureInfo.InvariantCulture); }));
                    //Limit
                    var BorrowLimit = (TotalBorrowUSD + BorrowVAI + VAIvault) / (TotalSupplyUSD * 0.6);
                    if (lLimit.InvokeRequired) lLimit.Invoke(new MethodInvoker(delegate { lLimit.Text = "Limit: " + BorrowLimit.ToString("P2", CultureInfo.InvariantCulture); }));
                    if (pbLimit.InvokeRequired) pbLimit.Invoke(new MethodInvoker(delegate { pbLimit.Value = (int)(BorrowLimit * 100); pbLimit.ForeColor = Color.FromArgb(249, 190, 86); }));
                    //Report
                    ReadSettings();
                    if (BorrowLimit * 100 > BorrowOver)
                    {
                        if (pbLimit.InvokeRequired)
                        {
                            pbLimit.Invoke(new MethodInvoker(delegate { pbLimit.ForeColor = Color.OrangeRed; }));
                        }
                        webclient.DownloadString(TelegramMSG + "Borrow Limit is OVER the limit! :( " + BorrowLimit.ToString("P2", CultureInfo.InvariantCulture));
                    }
                    else if (BorrowLimit * 100 < BorrowUnder)
                    {
                        if (pbLimit.InvokeRequired)
                        {
                            pbLimit.Invoke(new MethodInvoker(delegate { pbLimit.ForeColor = Color.GreenYellow; }));
                        }
                        webclient.DownloadString(TelegramMSG + "Borrow Limit is UNDER the limit! :) " + BorrowLimit.ToString("P2", CultureInfo.InvariantCulture));
                    }
                }
                catch { }

                await Task.Delay(TimeSpan.FromMinutes(RefreshMins));
            }
        }

        private void lSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {           
            var newFrm = new Form2();
            newFrm.Closed += delegate
            {
                ReadSettings();
            };
            newFrm.Show();            
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyVenusYield.Visible = true;
            }
        }

        private void notifyVenusYield_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyVenusYield.Visible = false;
        }
    }

    public class vTokenBalances
    { 
        public double PriceUSD { get; set; }
        public double Supply { get; set; }
        public double SupplyUSD { get; set; }
        public double Borrow { get; set; }
        public double BorrowUSD { get; set; }
    }

    public partial class BinancePriceChange
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("lastPrice")]
        public string LastPrice { get; set; }

        [JsonProperty("priceChangePercent")]
        public string PriceChangePercent { get; set; }

        public static BinancePriceChange FromJson(string json) => JsonConvert.DeserializeObject<BinancePriceChange>(json);
    }

    public partial class BSCscanTokenBalance
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        public static BSCscanTokenBalance FromJson(string json) => JsonConvert.DeserializeObject<BSCscanTokenBalance>(json);
    }

    public partial class VenusAPIvTokenService
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        public static VenusAPIvTokenService FromJson(string json) => JsonConvert.DeserializeObject<VenusAPIvTokenService>(json);
    }

    public partial class Data
    {
        [JsonProperty("venusRate")]
        public string VenusRate { get; set; }

        [JsonProperty("dailyVenus")]
        public string DailyVenus { get; set; }

        [JsonProperty("markets")]
        public List<Market> Markets { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }
    }

    public partial class Market
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("underlyingAddress")]
        public string UnderlyingAddress { get; set; }

        [JsonProperty("underlyingName")]
        public string UnderlyingName { get; set; }

        [JsonProperty("underlyingSymbol")]
        public string UnderlyingSymbol { get; set; }

        [JsonProperty("venusSpeeds")]
        public string VenusSpeeds { get; set; }

        [JsonProperty("borrowerDailyVenus")]
        public string BorrowerDailyVenus { get; set; }

        [JsonProperty("supplierDailyVenus")]
        public string SupplierDailyVenus { get; set; }

        [JsonProperty("venusBorrowIndex")]
        public string VenusBorrowIndex { get; set; }

        [JsonProperty("venusSupplyIndex")]
        public string VenusSupplyIndex { get; set; }

        [JsonProperty("borrowRatePerBlock")]
        public string BorrowRatePerBlock { get; set; }

        [JsonProperty("supplyRatePerBlock")]
        public string SupplyRatePerBlock { get; set; }

        [JsonProperty("exchangeRate")]
        public string ExchangeRate { get; set; }

        [JsonProperty("underlyingPrice")]
        public string UnderlyingPrice { get; set; }

        [JsonProperty("totalBorrows")]
        public string TotalBorrows { get; set; }

        [JsonProperty("totalBorrows2")]
        public string TotalBorrows2 { get; set; }

        [JsonProperty("totalBorrowsUsd")]
        public string TotalBorrowsUsd { get; set; }

        [JsonProperty("totalSupply")]
        public string TotalSupply { get; set; }

        [JsonProperty("totalSupply2")]
        public string TotalSupply2 { get; set; }

        [JsonProperty("totalSupplyUsd")]
        public string TotalSupplyUsd { get; set; }

        [JsonProperty("cash")]
        public string Cash { get; set; }

        [JsonProperty("totalReserves")]
        public string TotalReserves { get; set; }

        [JsonProperty("reserveFactor")]
        public string ReserveFactor { get; set; }

        [JsonProperty("collateralFactor")]
        public string CollateralFactor { get; set; }

        [JsonProperty("borrowApy")]
        public string BorrowApy { get; set; }

        [JsonProperty("supplyApy")]
        public string SupplyApy { get; set; }

        [JsonProperty("borrowVenusApy")]
        public string BorrowVenusApy { get; set; }

        [JsonProperty("supplyVenusApy")]
        public string SupplyVenusApy { get; set; }

        [JsonProperty("liquidity")]
        public string Liquidity { get; set; }

        [JsonProperty("tokenPrice")]
        public string TokenPrice { get; set; }

        [JsonProperty("totalDistributed")]
        public long TotalDistributed { get; set; }

        [JsonProperty("totalDistributed2")]
        public string TotalDistributed2 { get; set; }

        [JsonProperty("lastCalculatedBlockNumber")]
        public long LastCalculatedBlockNumber { get; set; }

        [JsonProperty("borrowerCount")]
        public long BorrowerCount { get; set; }

        [JsonProperty("supplierCount")]
        public long SupplierCount { get; set; }
    }

    public partial class Request
    {
        [JsonProperty("addresses")]
        public List<object> Addresses { get; set; }
    }
}
