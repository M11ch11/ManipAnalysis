USE ["DataBaseName"];

CREATE TABLE dbo._velocity_data_normalized (
  id         int IDENTITY NOT NULL, 
  trial_id   int NOT NULL, 
  time_stamp datetime2 NOT NULL, 
  velocity_x float NOT NULL, 
  velocity_y float NOT NULL, 
  velocity_z float NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._velocity_data_filtered (
  id         int IDENTITY NOT NULL, 
  trial_id   int NOT NULL, 
  time_stamp datetime2 NOT NULL, 
  velocity_x float NOT NULL, 
  velocity_y float NOT NULL, 
  velocity_z float NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._trial_information(
  id                      int IDENTITY NOT NULL, 
  faulty_trial            bit NOT NULL, 
  butterworth_filterOrder int NOT NULL, 
  butterworth_cutOffFreq  int NOT NULL, 
  velocity_trim_threshold int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._trial (
  id                       int IDENTITY NOT NULL, 
  subject_id               int NOT NULL, 
  study_id                 int NOT NULL, 
  group_id                 int NOT NULL, 
  is_catch_trial_id        int NOT NULL, 
  szenario_id              int NOT NULL, 
  szenario_trial_number_id int NOT NULL, 
  target_id                int NOT NULL, 
  target_trial_number_id   int NOT NULL, 
  measure_file_id          int NOT NULL, 
  trial_information_id     int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._target_trial_number (
  id                  int IDENTITY NOT NULL, 
  target_trial_number int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._target (
  id            int IDENTITY NOT NULL, 
  target_number int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._szenario_trial_number (
  id                    int IDENTITY NOT NULL, 
  szenario_trial_number int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._szenario_mean_time_data (
  szenario_mean_time_id  int NOT NULL, 
  szenario_mean_time     time NOT NULL, 
  szenario_mean_time_std time NOT NULL, 
  PRIMARY KEY (szenario_mean_time_id));
  
CREATE TABLE dbo._szenario_mean_time (
  id              int IDENTITY NOT NULL, 
  subject_id      int NOT NULL, 
  study_id        int NOT NULL, 
  group_id        int NOT NULL, 
  target_id       int NOT NULL, 
  szenario_id     int NOT NULL, 
  measure_file_id int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._szenario (
  id            int IDENTITY NOT NULL, 
  szenario_name varchar(max) NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._subject (
  id           int IDENTITY NOT NULL, 
  subject_name varchar(max) NOT NULL, 
  subject_id   varchar(max) NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._study (
  id         int IDENTITY NOT NULL, 
  study_name varchar(max) NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._statistic_data (
  trial_id                                int NOT NULL, 
  velocity_vector_correlation             float NOT NULL, 
  trajectory_length_abs                   float NOT NULL, 
  trajectory_length_ratio_baseline        float NOT NULL, 
  perpendicular_displacement_300ms_abs    float NOT NULL, 
  maximal_perpendicular_displacement_abs  float NOT NULL, 
  mean_perpendicular_displacement_abs     float NOT NULL, 
  perpendicular_displacement_300ms_sign   float NOT NULL, 
  maximal_perpendicular_displacement_sign float NOT NULL, 
  enclosed_area                           float NOT NULL, 
  rmse                                    float NOT NULL, 
  CONSTRAINT PK___statist__3213E83FAADB2263 
    PRIMARY KEY (trial_id));
	
CREATE TABLE dbo._measure_file (
  id            int IDENTITY NOT NULL, 
  creation_time datetime2 NOT NULL, 
  file_hash     varchar(max) NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._measure_data_raw (
  id                   int IDENTITY NOT NULL, 
  trial_id             int NOT NULL, 
  time_stamp           datetime2 NOT NULL, 
  force_actual_x       float NOT NULL, 
  force_actual_y       float NOT NULL, 
  force_actual_z       float NOT NULL, 
  force_nominal_x      float NOT NULL, 
  force_nominal_y      float NOT NULL, 
  force_nominal_z      float NOT NULL, 
  force_moment_x       float NOT NULL, 
  force_moment_y       float NOT NULL, 
  force_moment_z       float NOT NULL, 
  position_cartesian_x float NOT NULL, 
  position_cartesian_y float NOT NULL, 
  position_cartesian_z float NOT NULL, 
  position_status      int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._measure_data_normalized (
  id                   int IDENTITY NOT NULL, 
  trial_id             int NOT NULL, 
  time_stamp           datetime2 NOT NULL, 
  force_actual_x       float NOT NULL, 
  force_actual_y       float NOT NULL, 
  force_actual_z       float NOT NULL, 
  force_nominal_x      float NOT NULL, 
  force_nominal_y      float NOT NULL, 
  force_nominal_z      float NOT NULL, 
  force_moment_x       float NOT NULL, 
  force_moment_y       float NOT NULL, 
  force_moment_z       float NOT NULL, 
  position_cartesian_x float NOT NULL, 
  position_cartesian_y float NOT NULL, 
  position_cartesian_z float NOT NULL, 
  position_status      int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._measure_data_filtered (
  id                   int IDENTITY NOT NULL, 
  trial_id             int NOT NULL, 
  time_stamp           datetime2 NOT NULL, 
  force_actual_x       float NOT NULL, 
  force_actual_y       float NOT NULL, 
  force_actual_z       float NOT NULL, 
  force_nominal_x      float NOT NULL, 
  force_nominal_y      float NOT NULL, 
  force_nominal_z      float NOT NULL, 
  force_moment_x       float NOT NULL, 
  force_moment_y       float NOT NULL, 
  force_moment_z       float NOT NULL, 
  position_cartesian_x float NOT NULL, 
  position_cartesian_y float NOT NULL, 
  position_cartesian_z float NOT NULL, 
  position_status      int NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._is_catch_trial (
  id             int IDENTITY NOT NULL, 
  is_catch_trial bit NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._group (
  id         int IDENTITY NOT NULL, 
  group_name varchar(max) NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._baseline_data (
  id                            int IDENTITY NOT NULL, 
  baseline_id                   int NOT NULL, 
  pseudo_time_stamp             datetime2 NOT NULL, 
  baseline_position_cartesian_x float NOT NULL, 
  baseline_position_cartesian_y float NOT NULL, 
  baseline_position_cartesian_z float NOT NULL, 
  baseline_velocity_x           float NOT NULL, 
  baseline_velocity_y           float NOT NULL, 
  baseline_velocity_z           float NOT NULL, 
  PRIMARY KEY (id));
  
CREATE TABLE dbo._baseline (
  id              int IDENTITY NOT NULL, 
  subject_id      int NOT NULL, 
  study_id        int NOT NULL, 
  group_id        int NOT NULL, 
  target_id       int NOT NULL, 
  szenario_id     int NOT NULL, 
  measure_file_id int NOT NULL, 
  PRIMARY KEY (id));
  
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial350721 FOREIGN KEY (trial_information_id) REFERENCES dbo._trial_information (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial142237 FOREIGN KEY (target_trial_number_id) REFERENCES dbo._target_trial_number (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial577625 FOREIGN KEY (target_id) REFERENCES dbo._target (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._szenario_mean_time ADD CONSTRAINT FK_szenario_802035 FOREIGN KEY (target_id) REFERENCES dbo._target (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._baseline ADD CONSTRAINT FK_baseline275954 FOREIGN KEY (target_id) REFERENCES dbo._target (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial775692 FOREIGN KEY (szenario_trial_number_id) REFERENCES dbo._szenario_trial_number (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial771192 FOREIGN KEY (szenario_id) REFERENCES dbo._szenario (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._szenario_mean_time ADD CONSTRAINT FK_szenario_995602 FOREIGN KEY (szenario_id) REFERENCES dbo._szenario (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._baseline ADD CONSTRAINT FK_baseline502069 FOREIGN KEY (szenario_id) REFERENCES dbo._szenario (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial865566 FOREIGN KEY (subject_id) REFERENCES dbo._subject (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._szenario_mean_time ADD CONSTRAINT FK_szenario_89977 FOREIGN KEY (subject_id) REFERENCES dbo._subject (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._baseline ADD CONSTRAINT FK_baseline563895 FOREIGN KEY (subject_id) REFERENCES dbo._subject (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial389008 FOREIGN KEY (study_id) REFERENCES dbo._study (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._szenario_mean_time ADD CONSTRAINT FK_szenario_613418 FOREIGN KEY (study_id) REFERENCES dbo._study (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._baseline ADD CONSTRAINT FK_baseline87337 FOREIGN KEY (study_id) REFERENCES dbo._study (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial993785 FOREIGN KEY (measure_file_id) REFERENCES dbo._measure_file (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._szenario_mean_time ADD CONSTRAINT FK_szenario_781803 FOREIGN KEY (measure_file_id) REFERENCES dbo._measure_file (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._baseline ADD CONSTRAINT FK_baseline692114 FOREIGN KEY (measure_file_id) REFERENCES dbo._measure_file (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial522099 FOREIGN KEY (is_catch_trial_id) REFERENCES dbo._is_catch_trial (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._trial ADD CONSTRAINT FK_trial196049 FOREIGN KEY (group_id) REFERENCES dbo._group (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._szenario_mean_time ADD CONSTRAINT FK_szenario_999951 FOREIGN KEY (group_id) REFERENCES dbo._group (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._baseline ADD CONSTRAINT FK_baseline497720 FOREIGN KEY (group_id) REFERENCES dbo._group (id) ON UPDATE Cascade ON DELETE Cascade;
ALTER TABLE dbo._szenario_mean_time_data ADD CONSTRAINT FK_szenario_120958 FOREIGN KEY (szenario_mean_time_id) REFERENCES dbo._szenario_mean_time (id);
ALTER TABLE dbo._baseline_data ADD CONSTRAINT FK_baseline_144744 FOREIGN KEY (baseline_id) REFERENCES dbo._baseline (id);
ALTER TABLE dbo._statistic_data ADD CONSTRAINT FK_statistic531216 FOREIGN KEY (trial_id) REFERENCES dbo._trial (id);
ALTER TABLE dbo._velocity_data_normalized ADD CONSTRAINT FK_velocity_768888 FOREIGN KEY (trial_id) REFERENCES dbo._trial (id);
ALTER TABLE dbo._velocity_data_filtered ADD CONSTRAINT FK_velocity_623475 FOREIGN KEY (trial_id) REFERENCES dbo._trial (id);
ALTER TABLE dbo._measure_data_normalized ADD CONSTRAINT FK_measure_d118830 FOREIGN KEY (trial_id) REFERENCES dbo._trial (id);
ALTER TABLE dbo._measure_data_filtered ADD CONSTRAINT FK_measure_d167413 FOREIGN KEY (trial_id) REFERENCES dbo._trial (id);
ALTER TABLE dbo._measure_data_raw ADD CONSTRAINT FK_measure_d697812 FOREIGN KEY (trial_id) REFERENCES dbo._trial (id);

