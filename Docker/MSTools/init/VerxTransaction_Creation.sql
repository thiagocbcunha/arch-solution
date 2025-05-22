USE [master]
GO
/****** Object:  Database [VerxTransaction]    Script Date: 07/06/2024 18:05:37 ******/
CREATE DATABASE [VerxTransaction]
 CONTAINMENT = NONE
 ON PRIMARY 
( NAME = N'VerxTransaction', FILENAME = N'/var/opt/mssql/data/VerxTransaction.mdf' , SIZE = 10240KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'VerxTransaction_log', FILENAME = N'/var/opt/mssql/data/VerxTransaction_log.ldf' , SIZE = 2048KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)