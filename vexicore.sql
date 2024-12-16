-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.7.43-log - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             12.5.0.6677
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for vexicore
DROP DATABASE IF EXISTS `vexicore`;
CREATE DATABASE IF NOT EXISTS `vexicore` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_turkish_ci */;
USE `vexicore`;

-- Dumping structure for table vexicore.dc-rec
DROP TABLE IF EXISTS `dc-rec`;
CREATE TABLE IF NOT EXISTS `dc-rec` (
  `last-id` bigint(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci COMMENT='Discord Recomendations';

-- Dumping data for table vexicore.dc-rec: ~0 rows (approximately)
DELETE FROM `dc-rec`;

-- Dumping structure for table vexicore.dc-rec-likes
DROP TABLE IF EXISTS `dc-rec-likes`;
CREATE TABLE IF NOT EXISTS `dc-rec-likes` (
  `rid` varchar(32) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `uid` bigint(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

-- Dumping data for table vexicore.dc-rec-likes: ~0 rows (approximately)
DELETE FROM `dc-rec-likes`;

-- Dumping structure for table vexicore.dc-recomendations
DROP TABLE IF EXISTS `dc-recomendations`;
CREATE TABLE IF NOT EXISTS `dc-recomendations` (
  `id` varchar(32) COLLATE utf8mb4_turkish_ci NOT NULL DEFAULT '',
  `data` varchar(500) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `owner` bigint(20) DEFAULT NULL,
  `mid` bigint(20) DEFAULT NULL,
  `status` smallint(1) DEFAULT '0',
  `date` date DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

-- Dumping data for table vexicore.dc-recomendations: ~0 rows (approximately)
DELETE FROM `dc-recomendations`;

-- Dumping structure for table vexicore.sessions
DROP TABLE IF EXISTS `sessions`;
CREATE TABLE IF NOT EXISTS `sessions` (
  `sid` varchar(32) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `uid` varchar(32) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `create` datetime DEFAULT NULL,
  `expire` datetime DEFAULT NULL,
  `hwid` varchar(256) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `ipv4` varchar(32) COLLATE utf8mb4_turkish_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

-- Dumping data for table vexicore.sessions: ~1 rows (approximately)
DELETE FROM `sessions`;
INSERT INTO `sessions` (`sid`, `uid`, `create`, `expire`, `hwid`, `ipv4`) VALUES
	('sysot5n4wkxyzptnetm2fpv24ro7a9tg', 'tko8y9hab6cm66jdqg9zzv9jlslvbyd1', '2023-09-21 17:25:55', '2023-10-21 17:25:55', 'QkZFQkZCRkYwMDAyMDZBNy0xNTA2NDY5MTA1MDY5NDEtNTAwMjZCNzI1MzA5QzBFNA==', '::1');

-- Dumping structure for table vexicore.tickets
DROP TABLE IF EXISTS `tickets`;
CREATE TABLE IF NOT EXISTS `tickets` (
  `uid` bigint(20) DEFAULT NULL,
  `cid` bigint(20) DEFAULT NULL,
  `email` varchar(100) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `status` smallint(1) DEFAULT NULL COMMENT '0 - Açık\r\n1 - Kapalı\r\n2 - Beklemede'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

-- Dumping data for table vexicore.tickets: ~0 rows (approximately)
DELETE FROM `tickets`;

-- Dumping structure for table vexicore.user-plans
DROP TABLE IF EXISTS `user-plans`;
CREATE TABLE IF NOT EXISTS `user-plans` (
  `uid` varchar(32) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `plan-id` varchar(32) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `start-date` datetime DEFAULT NULL,
  `expire-date` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

-- Dumping data for table vexicore.user-plans: ~0 rows (approximately)
DELETE FROM `user-plans`;

-- Dumping structure for table vexicore.users
DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `uid` varchar(32) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `username` varchar(30) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `pwd` varchar(100) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `email` varchar(150) COLLATE utf8mb4_turkish_ci DEFAULT NULL,
  `register-date` datetime DEFAULT NULL,
  `gsm` int(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_turkish_ci;

-- Dumping data for table vexicore.users: ~1 rows (approximately)
DELETE FROM `users`;
INSERT INTO `users` (`uid`, `username`, `pwd`, `email`, `register-date`, `gsm`) VALUES
	('tko8y9hab6cm66jdqg9zzv9jlslvbyd1', 'swls', 'N1c271MuZQDO0uUqiWGKqEyaN+IjNvcnxxkB+Po41wz35PhSjMK162owWQauZ9bXzxmlsxQsFVAt9sE68bLmtw==', 'burkatunc1826@gmail.com', '2023-08-29 22:44:46', 0);

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
