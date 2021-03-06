﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QTLProject.Types;

namespace QTLProject
{
    public class DataGeneratorController
    {
        #region Fields
        public GenomeOrganization go = null;
        
        private int nChr;
        private OrganismType type;
        public Individ mother;
        public Individ father;
        public Individ offSpring;
        public Population pop;

        #endregion Fields
        /*Notes - 
         * DefineChromosomeLength - Should be run first to define the length since the next method assumes that the length was already defined (DefineChromosomePositions).
         * 
         */
         //My First Commit
        #region Constructor
        public DataGeneratorController(int _nChr, OrganismType _type)
        {
            go = new GenomeOrganization();
            nChr = _nChr;
            type = _type;
           
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// This is  method defines the location of the loci on the chromosome
        /// </summary>
        /// <param name="dBetweenMarkers_set"></param>
        /// <param name="nMarkersGrouped"></param>
        /// <param name="dMarkersGrouped"></param>
        /// <param name="indexAnimal"></param>
        public void DefineChromosomePositions(double dBetweenMarkers_set ,long nMarkersGrouped , double dMarkersGrouped , int indexAnimal )
        {
            int index=1;

            double coorPrev=0.0, d, coorStartOnCurrentChr=0.0, dBetweenMarkers = 2.0;
            long j, nMarkersMax, nOnChr=11;

            if (dBetweenMarkers_set > 0)
            {
                //distance between markers
                dBetweenMarkers = dBetweenMarkers_set;
            }

            if (index == 0)
            {
                //amount of markers
                nMarkersMax = nChr * nOnChr;
            }
            else
            {
                //amount of markers
                nMarkersMax = nChr * (int)(500 / dBetweenMarkers);
            }

            for(int i = 0; i < nChr; i++)
            {
                if (index == 0)
                {
                    d = go.Chromosome[i].LenGenetcM / (nOnChr - 1);
                }
                else
                {
                    d = dBetweenMarkers;
                    nOnChr=(int)(go.Chromosome[i].LenGenetcM/d)+1;
                }
                //Line 195 is this correct
                if (go.Chromosome[i].Id > 1)
                {
                    coorPrev = coorPrev + 1000 - d;
                }
                else
                {
                    coorPrev = -d;
                }
                for (j = 0; j < nOnChr; j++)
                {

                    Locus loci = new Locus();
                    //default location
                    loci.Position.Chromosome = go.Chromosome[i];
                    loci.Position.PositionChrGenetic = coorPrev + d;

                    if(j>1 && j<= dMarkersGrouped)
                    {
                        loci.Position.PositionChrGenetic = coorPrev + dMarkersGrouped;
                    }
                    if (j == 1)
                    {
                        coorStartOnCurrentChr = loci.Position.PositionChrGenetic;
                    }
                    if (j == nOnChr)
                    {
                        loci.Position.PositionChrGenetic = coorStartOnCurrentChr + go.Chromosome[i].LenGenetcM;
                    }
                    //Line 215 not clear
                    // go.Chromosome[i]loci.Position.PositionChrGenetic - coorStartOnCurrentChr;

                    loci.Name = "loc_" + j;
                    loci.Id = i;
                    coorPrev = loci.Position.PositionChrGenetic;

                    go.Chromosome[i].Locus.Add(loci);
                }    
            
            
            }

        }

        /// <summary>
        /// This method defines the length of the chromosomes
        /// </summary>
        public void DefineChromosomeLength()
        {
            
            switch (type)
            {
                case OrganismType.Drosophila:
                    go = genereateDrosophila(go,nChr);
                    break;


                case OrganismType.PseudoWheat:
                    go = generatePseudoWheat(go,nChr);
                    break;



                default:
                    //error organisim is not supported
                    break;

            }
            

        }
        /// <summary>
        /// Defines the parental haplotypes according to the recombination type
        /// </summary>
        /// <param name="recType"></param>
        public void DefineParentalHaplotypes(RecombinationType recType)
        {

             mother = new Individ();
             father = new Individ();
             Random s_Random = new Random();
            switch (recType)
            {
                case RecombinationType.Backcross:
                  for(int i = 0; i < mother.Haplotype0.Length; i++)
                    {
                        mother.Haplotype0[i] = 0;
                    }
                    for (int i = 0; i < father.Haplotype0.Length; i++)
                    {
                        father.Haplotype0[i] = 1;
                    }
                    break;

                case RecombinationType.BackcrossWithNoise:
                    for (int i = 0; i < mother.Haplotype0.Length; i++)
                    {
                        int perCent = s_Random.Next(0, 100);
                        if (perCent < 20)
                        {
                            mother.Haplotype0[i] = 1;
                        }
                        else
                        {
                            mother.Haplotype0[i] = 0;
                        }
                       
                    }
                    for (int i = 0; i < father.Haplotype0.Length; i++)
                    {
                        int perCent = s_Random.Next(0, 100);
                        if (perCent < 20)
                        {
                            mother.Haplotype0[i] = 0;
                        }
                        else
                        {
                            mother.Haplotype0[i] = 1;
                        }
                    }
                    break;
            }
          
        }

        /// <summary>
        /// Set the Offsprings haplotypes  in a certain locus index
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="ILocus"></param>
        public void GenotypeLocusSet(int a0,int a1,long ILocus)
        {
            offSpring.Haplotype0[ILocus] = a0;
            offSpring.Haplotype1[ILocus] = a1;
        }
        /// <summary>
        /// Get the offspring halpotypes according to Locus index
        /// </summary>
        /// <param name="ILocus"></param>
        /// <returns></returns>
        public IndividualHapl GenotypeLocusGSet(long ILocus)
        {
            IndividualHapl hapl = new IndividualHapl();
            hapl.a0=offSpring.Haplotype0[ILocus];
            hapl.a1=offSpring.Haplotype1[ILocus];
            return hapl;
        }


        public void SimulateRecombination(int amountOfIndivids)
        {
            //create population of  200 children -individulas
            pop = new Population();
            for(int i = 0; i < amountOfIndivids; i++)
            {
                //the same parents
                Individ offSpring = new Individ();
                offSpring.Parent0 = mother;
                offSpring.Parent1 = father;
                pop.Individ.Add(offSpring);
            }


            //calculate recombination pathways
            //FusionSimple()
            //gameta_get()
        }

        #endregion Public Methods

        #region Private Methods

        private void FusionSimple()
        {

        }

        private void gameta_get()
        {

        }
        private GenomeOrganization genereateDrosophila(GenomeOrganization go, int nChr)
        {
            double coef = 1;
            int drosophilaConst1 = 75, drosophilaConst2 = 107, drosophilaConst3 = 110;
            Chromosome ch = new Chromosome
            {
                Id = 0,
                LenGenetcM = drosophilaConst1 * coef,
                BRecInFemales = true,
                BGender = true
            };

            go.Chromosome.Add(ch);

            Chromosome ch1 = new Chromosome
            {
                Id = 1,
                LenGenetcM = drosophilaConst2 * coef,
                BRecInFemales = true,
                BGender = true
            };

            go.Chromosome.Add(ch1);

            Chromosome ch2 = new Chromosome
            {
                Id = 3,
                LenGenetcM = drosophilaConst3 * coef,
                BRecInFemales = true,
                BGender = true
            };

            go.Chromosome.Add(ch2);



            return go;
        }


        private GenomeOrganization generatePseudoWheat(GenomeOrganization go, int nChr)
        {



            return go;
        }
        #endregion Private Methods
    }

}
