CREATE DATABASE IF NOT EXISTS `renligou` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `renligou`;
SET NAMES utf8mb4;

-- 事件存储表
DROP TABLE IF EXISTS `_events`;
CREATE TABLE IF NOT EXISTS `_events`
(
    `id`          BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'ID',
    `category`    ENUM ('APPLICATION', 'DOMAIN')            NOT NULL COMMENT '事件分类（APPLICATION=应用层时间，DOMAIN=领域事件）',
    `source_type` VARCHAR(255)                              NOT NULL COMMENT '事件源类型',
    `source_id`   VARCHAR(64)                               NOT NULL COMMENT '事件源ID',
    `event_type`  VARCHAR(255)                              NOT NULL COMMENT '事件类型',
    `payload`     JSON                                      NOT NULL COMMENT '事件变动内容',
    `status`      ENUM ('NEW', 'SENDING', 'SENT', 'FAILED') NOT NULL DEFAULT 'NEW' COMMENT '事件状态（NEW=新建，SENDING=发送中，SENT=已发送，FAILED=发送失败）',
    `retry_count` INT                                       NOT NULL DEFAULT 0 COMMENT '重试次数',
    `occurred_at` DATETIME(6)                               NOT NULL COMMENT '事件发生事件',
    `sent_at`     DATETIME(6)                               NULL COMMENT '事件发送事件',
    `created_at`  DATETIME(6)                               NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
    `updated_at`  DATETIME(6)                               NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
    `version`     BIGINT                                    NOT NULL DEFAULT 0 COMMENT '乐观锁版本号',
    INDEX `idx_outbox_status_created` (`status`, `created_at`) USING BTREE,
    INDEX `idx_outbox_source` (`source_type`, `source_id`) USING BTREE
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT '事件存储表(Outbox)';

-- 消费消息表
DROP TABLE IF EXISTS `_processed_messages`;
CREATE TABLE IF NOT EXISTS `_processed_messages`
(
    `id`            BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT "ID",
    `consumer_name` VARCHAR(255) NOT NULL COMMENT "消费者名称",
    `message_id`    VARCHAR(128) NOT NULL COMMENT "消息ID",
    `processed_at`  DATETIME(6)  NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT "消费时间",
    UNIQUE KEY `uk_consumer_message` (`consumer_name`, `message_id`)
) ENGINE = InnoDB COMMENT ='消费消息表';

-- 中国行政区划表
DROP TABLE IF EXISTS `regions`;
CREATE TABLE `regions`
(
    `id`           BIGINT UNSIGNED     NOT NULL COMMENT 'ID',
    `parent_id`    BIGINT UNSIGNED     NOT NULL DEFAULT '0' COMMENT '父',
    `region_level` TINYINT(1) UNSIGNED NOT NULL DEFAULT '1' COMMENT '级别 1:省 2:市 3:区/县 4:镇/街道 5:村/社区',
    `postal_code`  CHAR(6)             NOT NULL DEFAULT '' COMMENT '邮政编码',
    `area_code`    CHAR(6)             NOT NULL DEFAULT '' COMMENT '区号',
    `region_name`  VARCHAR(50)         NOT NULL DEFAULT '' COMMENT '区划名称',
    `name_pinyin`  VARCHAR(255)        NOT NULL DEFAULT '' COMMENT '区划名称拼音',
    `short_name`   VARCHAR(50)         NOT NULL DEFAULT '' COMMENT '简称',
    `merge_name`   VARCHAR(255)        NOT NULL DEFAULT '' COMMENT '合并名称',
    `longitude`    DECIMAL(10, 6)      NOT NULL DEFAULT '0.000000' COMMENT '精度',
    `latitude`     DECIMAL(10, 6)      NOT NULL DEFAULT '0.000000' COMMENT '维度',
    PRIMARY KEY (`id`),
    KEY `idx_parent_id` (`parent_id`) USING BTREE
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='中国行政区划表';

-- 公司/子公司/分公司表
DROP TABLE IF EXISTS `companies`;
CREATE TABLE IF NOT EXISTS `companies`
(
    `id`                 BIGINT                                       NOT NULL PRIMARY KEY COMMENT 'ID',
    `company_type`       ENUM ('HEADQUARTER', 'BRANCH', 'SUBSIDIARY') NOT NULL COMMENT '公司类型: HEADQUARTER(总公司) / BRANCH(分公司) / SUBSIDIARY(子公司)',
    `company_code`       VARCHAR(64)                                  NOT NULL COMMENT '公司编码',
    `company_name`       VARCHAR(128)                                 NOT NULL COMMENT '公司名称',
    `company_short_name` VARCHAR(64)                                           DEFAULT '' COMMENT '公司名简称',
    `legal_person_name`  VARCHAR(64)                                           DEFAULT '' COMMENT '法人',
    `credit_code`        VARCHAR(32)                                           DEFAULT '' COMMENT '统一社会信用代码',
    `registered_address` VARCHAR(256)                                          DEFAULT '' COMMENT '注册地址',
    `remark`             VARCHAR(512)                                          DEFAULT '' COMMENT '备注',
    `created_at`         DATETIME                                     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at`         DATETIME                                     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',

    -- 地址信息（address）
    `province_id`        BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属省份ID',
    `province`           VARCHAR(50)                                  NOT NULL DEFAULT '' COMMENT '所属省份',
    `city_id`            BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属城市ID',
    `city`               VARCHAR(50)                                  NOT NULL DEFAULT '' COMMENT '所属城市',
    `district_id`        BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属区县ID',
    `district`           VARCHAR(50)                                  NOT NULL DEFAULT 0 COMMENT '所属区县',
    `completed_address`  VARCHAR(256)                                 NOT NULL DEFAULT '' COMMENT '完整地址',

    -- 启用状态（state)
    `enabled`            TINYINT(1)                                   NOT NULL DEFAULT 1 COMMENT '是否启用: 1=启用, 0=禁用',
    `effective_date`     DATE                                                  DEFAULT NULL COMMENT '生效日期',
    `expired_date`       DATE                                                  DEFAULT NULL COMMENT '失效日期',
    KEY `idx_company_code` (`company_code`) USING BTREE,
    UNIQUE KEY `uk_company_name` (`company_name`) USING BTREE
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='公司/子公司/分公司表';

-- 权限组
DROP TABLE IF EXISTS `permission_groups`;
CREATE TABLE IF NOT EXISTS `permission_groups`
(
    `id`           BIGINT PRIMARY KEY AUTO_INCREMENT COMMENT 'ID',
    `group_name`   VARCHAR(100) NOT NULL COMMENT '权限组名称',
    `display_name` VARCHAR(100) NOT NULL COMMENT '权限组显示名称',
    `description`  VARCHAR(255) NOT NULL DEFAULT '' COMMENT '权限组描述',
    `created_at`   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at`   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `deleted_at`   BIGINT       NOT NULL DEFAULT 0 COMMENT '软删除标记 0=未删除, 非0=已删除',
    UNIQUE INDEX `uk_group_name` (`group_name`) USING BTREE,
    UNIQUE INDEX `uk_display_name` (`display_name`) USING BTREE
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='权限组表';