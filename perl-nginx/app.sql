-- --------------------------------------------------------
-- Хост:                         127.0.0.1
-- Версия сервера:               5.7.29-log - MySQL Community Server (GPL)
-- Операционная система:         Win64
-- HeidiSQL Версия:              11.3.0.6295
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Дамп структуры базы данных app
CREATE DATABASE IF NOT EXISTS `app` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `app`;

-- Дамп структуры для таблица app.counters
CREATE TABLE IF NOT EXISTS `counters` (
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
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;

-- Дамп данных таблицы app.counters: ~6 rows (приблизительно)
/*!40000 ALTER TABLE `counters` DISABLE KEYS */;
INSERT INTO `counters` (`counter_id`, `usluga_ref`, `counter_name`, `serial`, `digits`, `precise`, `alt`) VALUES
	(1, 1, 'Счётчик 1-1', '123', 2, 3, '2022-04-21 12:21:54');
INSERT INTO `counters` (`counter_id`, `usluga_ref`, `counter_name`, `serial`, `digits`, `precise`, `alt`) VALUES
	(2, 2, 'Счётчик 2-2', '222', 5, 2, '2022-04-20 12:24:23');
INSERT INTO `counters` (`counter_id`, `usluga_ref`, `counter_name`, `serial`, `digits`, `precise`, `alt`) VALUES
	(3, 2, 'Счётчик 2-1', '122', 6, 3, '2022-04-20 12:24:31');
INSERT INTO `counters` (`counter_id`, `usluga_ref`, `counter_name`, `serial`, `digits`, `precise`, `alt`) VALUES
	(4, 1, 'Счётчик 1-2', '223', 5, 4, '2022-04-21 12:26:41');
INSERT INTO `counters` (`counter_id`, `usluga_ref`, `counter_name`, `serial`, `digits`, `precise`, `alt`) VALUES
	(5, 1, 'Счётчик 1-3', '124', 4, 2, '2022-04-21 14:39:24');
INSERT INTO `counters` (`counter_id`, `usluga_ref`, `counter_name`, `serial`, `digits`, `precise`, `alt`) VALUES
	(6, 20, 'Счётчик 3-1', '1', 7, 3, '2022-04-21 09:37:08');
INSERT INTO `counters` (`counter_id`, `usluga_ref`, `counter_name`, `serial`, `digits`, `precise`, `alt`) VALUES
	(16, 31, 'S1', 'SSS1', 6, 2, '2022-04-25 14:50:36');
/*!40000 ALTER TABLE `counters` ENABLE KEYS */;

-- Дамп структуры для представление app.counter_with_initials
-- Создание временной таблицы для обработки ошибок зависимостей представлений
CREATE TABLE `counter_with_initials` (
	`counter_id` INT(10) UNSIGNED NOT NULL,
	`usluga_ref` INT(10) UNSIGNED NOT NULL,
	`usluga_name` TEXT NOT NULL COLLATE 'utf8_general_ci',
	`counter_name` TEXT NOT NULL COLLATE 'utf8_general_ci',
	`serial` TEXT NOT NULL COLLATE 'utf8_general_ci',
	`digits` TINYINT(3) UNSIGNED NOT NULL,
	`precise` TINYINT(3) UNSIGNED NOT NULL,
	`start_date` DATE NULL,
	`init_value` DECIMAL(20,6) UNSIGNED NULL
) ENGINE=MyISAM;

-- Дамп структуры для таблица app.measures
CREATE TABLE IF NOT EXISTS `measures` (
  `m_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `counter_ref` int(10) unsigned NOT NULL,
  `m_date` date NOT NULL,
  `value` decimal(20,6) unsigned NOT NULL,
  `alt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`m_id`),
  UNIQUE KEY `counter_ref_m_date` (`counter_ref`,`m_date`) USING BTREE,
  KEY `counter_ref` (`counter_ref`) USING BTREE,
  CONSTRAINT `counter` FOREIGN KEY (`counter_ref`) REFERENCES `counters` (`counter_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8;

-- Дамп данных таблицы app.measures: ~17 rows (приблизительно)
/*!40000 ALTER TABLE `measures` DISABLE KEYS */;
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(1, 1, '2022-01-01', 55.550000, '2022-04-01 23:32:40');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(2, 1, '2022-03-23', 67.310000, '2022-04-19 17:09:01');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(3, 1, '2022-03-31', 69.330000, '2022-04-01 23:33:26');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(4, 2, '2022-02-11', 33.330000, '2022-04-01 23:33:48');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(5, 2, '2022-03-11', 44.440000, '2022-04-01 23:34:10');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(6, 3, '2022-01-01', 22.220000, '2022-04-01 23:34:41');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(7, 3, '2022-02-22', 33.330000, '2022-04-01 23:34:59');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(8, 4, '2022-01-30', 17.320000, '2022-04-01 23:35:29');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(9, 4, '2022-02-21', 24.230000, '2022-04-01 23:35:53');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(10, 5, '2022-01-02', 56.550000, '2022-04-19 10:45:51');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(11, 6, '1999-09-01', 123.000000, '2022-04-19 14:19:34');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(16, 1, '2022-03-24', 67.330000, '2022-04-19 17:09:14');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(17, 1, '2022-03-25', 67.340000, '2022-04-19 17:10:05');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(18, 5, '2022-01-03', 57.550000, '2022-04-19 17:11:51');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(19, 6, '2022-09-01', 123.000000, '2022-04-20 16:45:22');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(20, 4, '2022-03-21', 26.250000, '2022-04-21 14:31:36');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(23, 5, '2022-01-05', 56.550000, '2022-04-21 15:14:21');
INSERT INTO `measures` (`m_id`, `counter_ref`, `m_date`, `value`, `alt`) VALUES
	(27, 16, '2022-03-17', 11.330000, '2022-04-25 15:03:27');
/*!40000 ALTER TABLE `measures` ENABLE KEYS */;

-- Дамп структуры для таблица app.tariff
CREATE TABLE IF NOT EXISTS `tariff` (
  `tariff_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `usluga_ref` int(10) unsigned NOT NULL,
  `t_date` date NOT NULL,
  `price` decimal(20,6) unsigned NOT NULL,
  `alt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`tariff_id`),
  UNIQUE KEY `usluga_ref_t_date` (`usluga_ref`,`t_date`),
  CONSTRAINT `usluga1` FOREIGN KEY (`usluga_ref`) REFERENCES `usluga` (`usluga_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8;

-- Дамп данных таблицы app.tariff: ~8 rows (приблизительно)
/*!40000 ALTER TABLE `tariff` DISABLE KEYS */;
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(1, 1, '2022-03-25', 12.410000, '2022-04-21 14:29:41');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(2, 1, '2022-01-01', 11.120000, '2022-04-19 10:25:59');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(3, 2, '2022-01-01', 31.110000, '2022-04-01 23:21:09');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(4, 2, '2022-03-01', 31.330000, '2022-04-01 23:21:45');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(11, 20, '2021-12-01', 12.000000, '2022-04-20 16:09:43');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(12, 20, '2021-12-02', 13.000000, '2022-04-20 16:09:57');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(13, 20, '1990-12-02', 10.060000, '2022-04-21 11:32:19');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(17, 1, '2022-03-01', 12.000000, '2022-04-21 14:29:58');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(18, 31, '2022-04-25', 11.330000, '2022-04-25 14:42:54');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(19, 31, '2022-03-18', 11.110000, '2022-04-25 14:46:54');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(20, 31, '2022-03-19', 11.120000, '2022-04-25 14:47:12');
INSERT INTO `tariff` (`tariff_id`, `usluga_ref`, `t_date`, `price`, `alt`) VALUES
	(21, 31, '2022-03-17', 11.090000, '2022-04-25 14:49:00');
/*!40000 ALTER TABLE `tariff` ENABLE KEYS */;

-- Дамп структуры для таблица app.usluga
CREATE TABLE IF NOT EXISTS `usluga` (
  `usluga_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `usluga_name` text NOT NULL,
  `alt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`usluga_id`) USING BTREE,
  UNIQUE KEY `usluga_name` (`usluga_name`(64))
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8;

-- Дамп данных таблицы app.usluga: ~3 rows (приблизительно)
/*!40000 ALTER TABLE `usluga` DISABLE KEYS */;
INSERT INTO `usluga` (`usluga_id`, `usluga_name`, `alt`) VALUES
	(1, 'Услуга 1', '2022-04-21 09:36:32');
INSERT INTO `usluga` (`usluga_id`, `usluga_name`, `alt`) VALUES
	(2, 'Услуга 2', '2022-04-21 10:33:25');
INSERT INTO `usluga` (`usluga_id`, `usluga_name`, `alt`) VALUES
	(20, 'Услуга 3', '2022-04-21 09:36:07');
INSERT INTO `usluga` (`usluga_id`, `usluga_name`, `alt`) VALUES
	(31, 'Услуга 31', '2022-04-25 14:34:17');
/*!40000 ALTER TABLE `usluga` ENABLE KEYS */;

-- Дамп структуры для представление app.counter_with_initials
-- Удаление временной таблицы и создание окончательной структуры представления
DROP TABLE IF EXISTS `counter_with_initials`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `counter_with_initials` AS select `counters`.`counter_id` AS `counter_id`,`counters`.`usluga_ref` AS `usluga_ref`,`usluga`.`usluga_name` AS `usluga_name`,`counters`.`counter_name` AS `counter_name`,`counters`.`serial` AS `serial`,`counters`.`digits` AS `digits`,`counters`.`precise` AS `precise`,min(`measures`.`m_date`) AS `start_date`,min(`measures`.`value`) AS `init_value` from ((`counters` join `measures` on((`measures`.`counter_ref` = `counters`.`counter_id`))) join `usluga` on((`usluga`.`usluga_id` = `counters`.`usluga_ref`))) group by `counters`.`counter_id`;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
