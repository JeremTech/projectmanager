-- phpMyAdmin SQL Dump
-- version 4.8.3
-- https://www.phpmyadmin.net/
--
-- Hôte : 127.0.0.1
-- Généré le :  mar. 04 fév. 2020 à 22:19
-- Version du serveur :  10.1.36-MariaDB
-- Version de PHP :  7.2.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données :  `projectmanager`
--

-- --------------------------------------------------------

--
-- Structure de la table `appartient`
--

CREATE TABLE `appartient` (
  `ID_PROFIL` decimal(65,0) NOT NULL,
  `ID_TACHE` decimal(65,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Structure de la table `correspond`
--

CREATE TABLE `correspond` (
  `ID_PROJET` decimal(65,0) NOT NULL,
  `ID_TACHE` decimal(65,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Structure de la table `notif`
--

CREATE TABLE `notif` (
  `ID_NOTIF` decimal(65,0) NOT NULL,
  `IDUSERDESTINATION` decimal(65,0) DEFAULT NULL,
  `TITRE` text COLLATE utf8_bin,
  `DESCRIPTION` text COLLATE utf8_bin,
  `DATEENVOI` date DEFAULT NULL,
  `ISLU` decimal(1,0) DEFAULT NULL,
  `ISIMPOTANT` decimal(1,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Structure de la table `notifier`
--

CREATE TABLE `notifier` (
  `ID_PROFIL` decimal(65,0) NOT NULL,
  `ID_NOTIF` decimal(65,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Structure de la table `profil`
--

CREATE TABLE `profil` (
  `ID_PROFIL` decimal(65,0) NOT NULL,
  `NOM` text COLLATE utf8_bin,
  `PRENOM` text COLLATE utf8_bin,
  `MAIL` text COLLATE utf8_bin,
  `DATENAIS` date DEFAULT NULL,
  `DESCRIPTIION` text COLLATE utf8_bin,
  `MDP` text COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Structure de la table `projet`
--

CREATE TABLE `projet` (
  `ID_PROJET` decimal(65,0) NOT NULL,
  `NOM` text COLLATE utf8_bin,
  `DESCRIPTION` text COLLATE utf8_bin,
  `DATECREA` date DEFAULT NULL,
  `ETAT` text COLLATE utf8_bin,
  `DEADLINE` date DEFAULT NULL,
  `POURCENTAGE` decimal(3,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Structure de la table `role`
--

CREATE TABLE `role` (
  `ID_PROFIL` decimal(65,0) NOT NULL,
  `ID_PROJET` decimal(65,0) NOT NULL,
  `ROLE` text COLLATE utf8_bin
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Structure de la table `tache`
--

CREATE TABLE `tache` (
  `ID_TACHE` decimal(65,0) NOT NULL,
  `ETAT` text COLLATE utf8_bin,
  `DATEDEBUT` date DEFAULT NULL,
  `DATEFIN` date DEFAULT NULL,
  `ISMAJEUR` text COLLATE utf8_bin,
  `DESCRIPTION` text COLLATE utf8_bin,
  `POURCENTAGE` decimal(3,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Index pour les tables déchargées
--

--
-- Index pour la table `appartient`
--
ALTER TABLE `appartient`
  ADD PRIMARY KEY (`ID_PROFIL`,`ID_TACHE`),
  ADD KEY `FK_APPARTIENT2` (`ID_TACHE`);

--
-- Index pour la table `correspond`
--
ALTER TABLE `correspond`
  ADD PRIMARY KEY (`ID_PROJET`,`ID_TACHE`),
  ADD KEY `FK_CORRESPOND2` (`ID_TACHE`);

--
-- Index pour la table `notif`
--
ALTER TABLE `notif`
  ADD PRIMARY KEY (`ID_NOTIF`);

--
-- Index pour la table `notifier`
--
ALTER TABLE `notifier`
  ADD PRIMARY KEY (`ID_PROFIL`,`ID_NOTIF`),
  ADD KEY `FK_NOTIFIER2` (`ID_NOTIF`);

--
-- Index pour la table `profil`
--
ALTER TABLE `profil`
  ADD PRIMARY KEY (`ID_PROFIL`);

--
-- Index pour la table `projet`
--
ALTER TABLE `projet`
  ADD PRIMARY KEY (`ID_PROJET`);

--
-- Index pour la table `role`
--
ALTER TABLE `role`
  ADD PRIMARY KEY (`ID_PROFIL`,`ID_PROJET`),
  ADD KEY `FK_ROLE2` (`ID_PROJET`);

--
-- Index pour la table `tache`
--
ALTER TABLE `tache`
  ADD PRIMARY KEY (`ID_TACHE`);

--
-- Contraintes pour les tables déchargées
--

--
-- Contraintes pour la table `appartient`
--
ALTER TABLE `appartient`
  ADD CONSTRAINT `FK_APPARTIENT` FOREIGN KEY (`ID_PROFIL`) REFERENCES `profil` (`ID_PROFIL`),
  ADD CONSTRAINT `FK_APPARTIENT2` FOREIGN KEY (`ID_TACHE`) REFERENCES `tache` (`ID_TACHE`);

--
-- Contraintes pour la table `correspond`
--
ALTER TABLE `correspond`
  ADD CONSTRAINT `FK_CORRESPOND` FOREIGN KEY (`ID_PROJET`) REFERENCES `projet` (`ID_PROJET`),
  ADD CONSTRAINT `FK_CORRESPOND2` FOREIGN KEY (`ID_TACHE`) REFERENCES `tache` (`ID_TACHE`);

--
-- Contraintes pour la table `notifier`
--
ALTER TABLE `notifier`
  ADD CONSTRAINT `FK_NOTIFIER` FOREIGN KEY (`ID_PROFIL`) REFERENCES `profil` (`ID_PROFIL`),
  ADD CONSTRAINT `FK_NOTIFIER2` FOREIGN KEY (`ID_NOTIF`) REFERENCES `notif` (`ID_NOTIF`);

--
-- Contraintes pour la table `role`
--
ALTER TABLE `role`
  ADD CONSTRAINT `FK_ROLE` FOREIGN KEY (`ID_PROFIL`) REFERENCES `profil` (`ID_PROFIL`),
  ADD CONSTRAINT `FK_ROLE2` FOREIGN KEY (`ID_PROJET`) REFERENCES `projet` (`ID_PROJET`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */; 