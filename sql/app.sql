-- MySQL dump 10.13  Distrib 5.7.29, for Win64 (x86_64)
--
-- Host: localhost    Database: app
-- ------------------------------------------------------
-- Server version	5.7.29-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `app`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `app` /*!40100 DEFAULT CHARACTER SET utf8 */;

USE `app`;

--
-- Temporary table structure for view `counter_with_initials`
--

DROP TABLE IF EXISTS `counter_with_initials`;
/*!50001 DROP VIEW IF EXISTS `counter_with_initials`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE VIEW `counter_with_initials` AS SELECT 
 1 AS `counter_id`,
 1 AS `usluga_ref`,
 1 AS `usluga_name`,
 1 AS `counter_name`,
 1 AS `serial`,
 1 AS `digits`,
 1 AS `precise`,
 1 AS `start_date`,
 1 AS `init_value`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `counters`
--

DROP TABLE IF EXISTS `counters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `counters` (
  `counter_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `usluga_ref` int(10) unsigned NOT NULL,
  `counter_name` text NOT NULL,
  `serial` text NOT NULL,
  `digits` tinyint(3) unsigned NOT NULL,
  `precise` tinyint(3) unsigned NOT NULL,
  `alt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`counter_id`),
  UNIQUE KEY `counter_name_serial` (`counter_name`(128),`serial`(64)),
  KEY `tariff_ref` (`usluga_ref`) USING BTREE,
  CONSTRAINT `usluga` FOREIGN KEY (`usluga_ref`) REFERENCES `usluga` (`usluga_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `counters`
--

LOCK TABLES `counters` WRITE;
/*!40000 ALTER TABLE `counters` DISABLE KEYS */;
INSERT INTO `counters` VALUES (1,1,'Счётчик 1','123',6,3,'2022-04-01 20:23:20'),(2,2,'Счётчик 2','222',5,2,'2022-04-01 20:23:57'),(3,2,'Счётчик 1','122',6,3,'2022-04-01 20:24:24'),(4,1,'Счётчик 2','221',5,2,'2022-04-01 20:24:59'),(5,1,'Счётчик 3','124',6,3,'2022-04-19 07:45:51'),(6,20,'Счётчик','1',7,3,'2022-04-19 11:19:34');
/*!40000 ALTER TABLE `counters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `measures`
--

DROP TABLE IF EXISTS `measures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `measures` (
  `m_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `counter_ref` int(10) unsigned NOT NULL,
  `m_date` date NOT NULL,
  `value` decimal(20,6) unsigned NOT NULL,
  `alt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`m_id`),
  UNIQUE KEY `counter_ref_m_date` (`counter_ref`,`m_date`) USING BTREE,
  KEY `counter_ref` (`counter_ref`) USING BTREE,
  CONSTRAINT `counter` FOREIGN KEY (`counter_ref`) REFERENCES `counters` (`counter_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `measures`
--

LOCK TABLES `measures` WRITE;
/*!40000 ALTER TABLE `measures` DISABLE KEYS */;
INSERT INTO `measures` VALUES (1,1,'2022-01-01',55.550000,'2022-04-01 20:32:40'),(2,1,'2022-03-23',67.330000,'2022-04-01 20:33:03'),(3,1,'2022-03-31',69.330000,'2022-04-01 20:33:26'),(4,2,'2022-02-11',33.330000,'2022-04-01 20:33:48'),(5,2,'2022-03-11',44.440000,'2022-04-01 20:34:10'),(6,3,'2022-01-01',22.220000,'2022-04-01 20:34:41'),(7,3,'2022-02-22',33.330000,'2022-04-01 20:34:59'),(8,4,'2022-01-30',17.320000,'2022-04-01 20:35:29'),(9,4,'2022-02-21',24.230000,'2022-04-01 20:35:53'),(10,5,'2022-01-02',56.550000,'2022-04-19 07:45:51'),(11,6,'1999-09-01',123.000000,'2022-04-19 11:19:34');
/*!40000 ALTER TABLE `measures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tariff`
--

DROP TABLE IF EXISTS `tariff`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tariff` (
  `tariff_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `usluga_ref` int(10) unsigned NOT NULL,
  `t_date` date NOT NULL,
  `price` decimal(20,6) unsigned NOT NULL,
  `alt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`tariff_id`),
  KEY `usluga_ref` (`usluga_ref`),
  KEY `time_beg` (`t_date`),
  CONSTRAINT `usluga1` FOREIGN KEY (`usluga_ref`) REFERENCES `usluga` (`usluga_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tariff`
--

LOCK TABLES `tariff` WRITE;
/*!40000 ALTER TABLE `tariff` DISABLE KEYS */;
INSERT INTO `tariff` VALUES (1,1,'2022-03-01',12.120000,'2022-04-19 07:25:53'),(2,1,'2022-01-01',11.120000,'2022-04-19 07:25:59'),(3,2,'2022-01-01',31.110000,'2022-04-01 20:21:09'),(4,2,'2022-03-01',31.330000,'2022-04-01 20:21:45'),(7,20,'1978-08-01',22.300000,'2022-04-18 14:56:30'),(8,20,'1978-08-02',22.400000,'2022-04-18 14:56:40'),(9,20,'1978-08-03',22.500000,'2022-04-18 14:56:52');
/*!40000 ALTER TABLE `tariff` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usluga`
--

DROP TABLE IF EXISTS `usluga`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `usluga` (
  `usluga_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `usluga_name` text NOT NULL,
  `alt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`usluga_id`) USING BTREE,
  UNIQUE KEY `usluga_name` (`usluga_name`(64))
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usluga`
--

LOCK TABLES `usluga` WRITE;
/*!40000 ALTER TABLE `usluga` DISABLE KEYS */;
INSERT INTO `usluga` VALUES (1,'Тариф_1','2022-04-01 20:16:57'),(2,'Тариф_2','2022-04-01 20:17:19'),(20,'tariff_31','2022-04-18 16:07:43');
/*!40000 ALTER TABLE `usluga` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Current Database: `app`
--

USE `app`;

--
-- Final view structure for view `counter_with_initials`
--

/*!50001 DROP VIEW IF EXISTS `counter_with_initials`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `counter_with_initials` AS select `counters`.`counter_id` AS `counter_id`,`counters`.`usluga_ref` AS `usluga_ref`,`usluga`.`usluga_name` AS `usluga_name`,`counters`.`counter_name` AS `counter_name`,`counters`.`serial` AS `serial`,`counters`.`digits` AS `digits`,`counters`.`precise` AS `precise`,min(`measures`.`m_date`) AS `start_date`,min(`measures`.`value`) AS `init_value` from ((`counters` join `measures` on((`measures`.`counter_ref` = `counters`.`counter_id`))) join `usluga` on((`usluga`.`usluga_id` = `counters`.`usluga_ref`))) group by `counters`.`counter_id` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-04-19 14:25:48
