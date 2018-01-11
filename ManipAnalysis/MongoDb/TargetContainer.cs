namespace ManipAnalysis_v2.MongoDb
{
    public class TargetContainer
    {

        /// <summary>
        /// Number field is filled with the so called TRIAL:TP from the c3d file. Then later in the parsing this number is changed:
        /// In most of the abstract szenario definition if Target.Number was bigger than 10 (11 or more) it was just being left shifted,
        /// so that for example 25 becomes 5, 36 becomes 6, 35 also becomes 5 and so on... I don't know why though!
        /// In szenarios that had only 3 targets, it was substracted, so that it ends up in either range (1, 3) or (11, 13)
        /// This was due to the naming convention for the tp_table entries, but might change in the future!
        /// Check XML_Parser where trial.Target.Number is being set.
        /// In some szenarios with 8 targets it was somteimes also substracted so that it ends up in range (1, 8) and sometimes in (11, 18)...
        /// The target.Number is used when plotting data, aswell as when calculating statistics, baselines, szenarioMeanTimes, and also in the MongoDBWrapper...
        /// Therefore it should be important to set them correctly...
        /// In the end this field seems to only represent the EndTarget of the trial with a specific ID, so should also be identifieable by its positions...
        /// Each target can be described by its positions, now every trial with the same Endtarget will get the same trial.Target.Number
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// X Position of the target, is read from the dtp file and set by the parser.
        /// </summary>
        public double XPos { get; set; }
        /// <summary>
        /// Y Position of the target, is read from the dtp file and set by the parser.
        /// </summary>
        public double YPos { get; set; }
        /// <summary>
        /// Z Position of the target, is read from the dtp file and set by the parser. Should actually always be 0 anyways...
        /// </summary>
        public double ZPos { get; set; }
        /// <summary>
        /// Radius of the target, is read from the dtp file and set by the parser.
        /// </summary>
        public double Radius { get; set; }
    }
}