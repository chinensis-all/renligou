CREATE DATABASE IF NOT EXISTS `renligou` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `renligou`;
SET NAMES utf8mb4;

DROP TABLE IF EXISTS `regions`;
CREATE TABLE `regions`
(
    `id`           bigint unsigned                         NOT NULL COMMENT '行政编码',
    `parent_id`    bigint unsigned                         NOT NULL DEFAULT '0' COMMENT '上级行政编码',
    `region_level` tinyint unsigned                        NOT NULL DEFAULT '1' COMMENT '行政区划级别 1:省 2:市 3:区/县 4:镇/街道 5:村/社区',
    `postal_code`  char(6) COLLATE utf8mb4_unicode_ci      NOT NULL DEFAULT '' COMMENT '邮政编码',
    `area_code`    char(6) COLLATE utf8mb4_unicode_ci      NOT NULL DEFAULT '' COMMENT '区号',
    `region_name`  varchar(50) COLLATE utf8mb4_unicode_ci  NOT NULL DEFAULT '' COMMENT '行政区划名称',
    `name_pinyin`  varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '' COMMENT '行政区划名称拼音',
    `short_name`   varchar(50) COLLATE utf8mb4_unicode_ci  NOT NULL DEFAULT '' COMMENT '行政区划简称',
    `merge_name`   varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '' COMMENT '行政区划组合名称',
    `longitude`    decimal(10, 6)                          NOT NULL DEFAULT '0.000000' COMMENT '经度',
    `latitude`     decimal(10, 6)                          NOT NULL DEFAULT '0.000000' COMMENT '纬度',
    PRIMARY KEY (`id`),
    KEY `idx_parent_id` (`parent_id`) USING BTREE COMMENT '上级行政编码索引'
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='中国行政区划表';

DROP TABLE IF EXISTS `companies`;
CREATE TABLE IF NOT EXISTS `companies`
(
    `id`                 BIGINT       NOT NULL PRIMARY KEY COMMENT '主键ID',

    -- 公司基础信息
    `company_code`       VARCHAR(64)  NOT NULL COMMENT '公司编码（业务唯一）',
    `company_name`       VARCHAR(128) NOT NULL COMMENT '公司名称',
    `company_short_name` VARCHAR(64)           DEFAULT '' COMMENT '公司简称',

    -- 所在地址
    `province_id`        BIGINT                DEFAULT NULL COMMENT '省份ID',
    `province`           VARCHAR(50)           DEFAULT '' COMMENT '省份名称',
    `city_id`            BIGINT                DEFAULT NULL COMMENT '城市ID',
    `city`               VARCHAR(50)           DEFAULT '' COMMENT '城市名称',
    `district_id`        BIGINT                DEFAULT NULL COMMENT '区县ID',
    `district`           VARCHAR(50)           DEFAULT '' COMMENT '区县名称',
    `address`            VARCHAR(256)          DEFAULT '' COMMENT '详细地址',

    -- 公司类型
    `company_type`       VARCHAR(32)  NOT NULL COMMENT '公司类型: HEADQUARTER / BRANCH / SUBSIDIARY',

    -- 法人与合规
    `legal_person_name`  VARCHAR(64)           DEFAULT '' COMMENT '法人姓名',
    `credit_code`        VARCHAR(32)           DEFAULT '' COMMENT '统一社会信用代码',
    `registered_address` VARCHAR(256)          DEFAULT '' COMMENT '注册地址',

    -- HR 使用状态
    `enabled`            TINYINT(1)   NOT NULL DEFAULT 1 COMMENT '是否启用',
    `effective_date`     DATE                  DEFAULT NULL COMMENT '生效日期',
    `expired_date`       DATE                  DEFAULT NULL COMMENT '失效日期',

    -- 备注
    `remark`             VARCHAR(512)          DEFAULT '' COMMENT '备注',

    -- 审计字段
    `created_at`         DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at`         DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='公司/分公司表';