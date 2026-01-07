CREATE DATABASE IF NOT EXISTS `renligou` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `renligou`;
SET NAMES utf8mb4;

-- 可靠事件传递表
DROP TABLE IF EXISTS `_events`;
CREATE TABLE IF NOT EXISTS `_events`
(
    `id`             BIGINT  AUTO_INCREMENT PRIMARY KEY COMMENT '分布式唯一ID',
    `category`       ENUM('APPLICATION', 'DOMAIN')             NOT NULL COMMENT '事件类别 (APPLICATION=应用层事件, DOMAIN=领域层事件)',
    `source_type`    VARCHAR(255)                              NOT NULL COMMENT '来源类型',
    `source_id`      VARCHAR(64)                               NOT NULL COMMENT '来源ID',
    `event_type`     VARCHAR(255)                              NOT NULL COMMENT '事件类型',
    `payload`        JSON                                      NOT NULL COMMENT '事件负载',
    `status`         ENUM ('NEW', 'SENDING', 'SENT', 'FAILED') NOT NULL DEFAULT 'NEW' COMMENT '事件状态 (NEW=待发布, SENDING=发布中, SENT=已发布, FAILED=发布失败)',
    `retry_count`    INT                                       NOT NULL DEFAULT 0 COMMENT '重试次数',
    `occurred_at`    DATETIME(6)                               NOT NULL COMMENT '事件发生时间',
    `sent_at`        DATETIME(6)                               NULL     COMMENT '发送完成时间',
    `created_at`     DATETIME(6)                               NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
    `updated_at`     DATETIME(6)                               NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
    `version`        BIGINT                                    NOT NULL DEFAULT 0 COMMENT '乐观锁版本号',
    INDEX `idx_outbox_status_created` (`status`, `created_at`) USING BTREE,
    INDEX `idx_outbox_source` (`source_type`, `source_id`) USING BTREE
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT '可靠事件传递(Outbox)表';

-- 已处理消息表
DROP TABLE IF EXISTS `_processed_messages`;
CREATE TABLE IF NOT EXISTS `_processed_messages`
(
    `id`            BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT "ID",
    `consumer_name` VARCHAR(255) NOT NULL COMMENT "消费者名称",
    `message_id`    VARCHAR(128) NOT NULL COMMENT "消息ID",
    `processed_at`  DATETIME(6)  NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT "锁定时间",
    UNIQUE KEY `uk_consumer_message` (`consumer_name`, `message_id`)
) ENGINE = InnoDB COMMENT ='已处理消息表';

-- 中国行政区划表
DROP TABLE IF EXISTS `regions`;
CREATE TABLE `regions`
(
    `id`           BIGINT UNSIGNED                         NOT NULL COMMENT '行政编码',
    `parent_id`    BIGINT UNSIGNED                         NOT NULL DEFAULT '0' COMMENT '上级行政编码',
    `region_level` TINYINT(1) UNSIGNED                     NOT NULL DEFAULT '1' COMMENT '行政区划级别 1:省 2:市 3:区/县 4:镇/街道 5:村/社区',
    `postal_code`  CHAR(6)                                 NOT NULL DEFAULT '' COMMENT '邮政编码',
    `area_code`    CHAR(6)                                 NOT NULL DEFAULT '' COMMENT '区号',
    `region_name`  VARCHAR(50)                             NOT NULL DEFAULT '' COMMENT '行政区划名称',
    `name_pinyin`  VARCHAR(255)                            NOT NULL DEFAULT '' COMMENT '行政区划名称拼音',
    `short_name`   VARCHAR(50)                             NOT NULL DEFAULT '' COMMENT '行政区划简称',
    `merge_name`   VARCHAR(255)                            NOT NULL DEFAULT '' COMMENT '行政区划组合名称',
    `longitude`    DECIMAL(10, 6)                          NOT NULL DEFAULT '0.000000' COMMENT '经度',
    `latitude`     DECIMAL(10, 6)                          NOT NULL DEFAULT '0.000000' COMMENT '纬度',
    PRIMARY KEY (`id`),
    KEY `idx_parent_id` (`parent_id`) USING BTREE COMMENT '上级行政编码索引'
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='中国行政区划表';

-- 公司/分公司表
DROP TABLE IF EXISTS `companies`;
CREATE TABLE IF NOT EXISTS `companies`
(
    `id`                 BIGINT       NOT NULL PRIMARY KEY COMMENT '主键ID',
    `company_type`       VARCHAR(32)  NOT NULL COMMENT '公司类型: HEADQUARTER(总公司) / BRANCH(分公司) / SUBSIDIARY(子公司，独立法人实体)',
    `company_code`       VARCHAR(64)  NOT NULL COMMENT '公司编码（业务唯一）',
    `company_name`       VARCHAR(128) NOT NULL COMMENT '公司名称',
    `company_short_name` VARCHAR(64)           DEFAULT '' COMMENT '公司简称',
    `legal_person_name`  VARCHAR(64)           DEFAULT '' COMMENT '法人姓名',
    `credit_code`        VARCHAR(32)           DEFAULT '' COMMENT '统一社会信用代码',
    `registered_address` VARCHAR(256)          DEFAULT '' COMMENT '注册地址',
    `remark`             VARCHAR(512)          DEFAULT '' COMMENT '备注',
    `created_at`         DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at`         DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',

    -- 所在地址(address)
    `province_id`        BIGINT                DEFAULT NULL COMMENT '省份ID',
    `province`           VARCHAR(50)           DEFAULT '' COMMENT '省份名称',
    `city_id`            BIGINT                DEFAULT NULL COMMENT '城市ID',
    `city`               VARCHAR(50)           DEFAULT '' COMMENT '城市名称',
    `district_id`        BIGINT                DEFAULT NULL COMMENT '区县ID',
    `district`           VARCHAR(50)           DEFAULT '' COMMENT '区县名称',
    `complated_address`  VARCHAR(256)          DEFAULT '' COMMENT '详细地址',

    -- 启用状态及有效期(state)
    `enabled`            TINYINT(1)            NOT NULL DEFAULT 1 COMMENT '是否启用',
    `effective_date`     DATE                  DEFAULT NULL COMMENT '生效日期',
    `expired_date`       DATE                  DEFAULT NULL COMMENT '失效日期',
    KEY `idx_company_code` (`company_code`) USING BTREE，
    KEY `idx_company_name` (`company_name`) USING BTREE
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='公司/分公司表';