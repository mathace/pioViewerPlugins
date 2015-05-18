using System;
using System.Text;
using System.Windows.Forms;
using PioViewerApi.Plugin;
using PioViewerApi.Server;
using PioViewerApi.Util;

namespace SimplePlugin
{
    /// <summary>
    /// In this example we are introducing the basic solver queries. 
    /// We ask for a range, ev and equity of a player in a node
    /// </summary>
    public class RangeEVEquityPlugin : IServerPlugin
    {
        public string Name
        {
            get { return "Show Range EV Equity"; }
        }

        public string Category
        {
            get { return "Tutorial"; }
        }

        public bool IsGUIPlugin
        {
            get { return true; }
        }

        protected IServerPluginContext Context { get; private set; }

        public void Initialize(IServerPluginContext context)
        {
            this.Context = context;
        }

        public void Execute(IPluginProgressProvider progress)
        {
            //If the user selects plugin from the menu we can ask controller about currently selected node.
            IServerNode currentlySelectedNode = Context.Controller.SelectedNode;
            if (currentlySelectedNode == null)
            {
                MessageBox.Show("There is no node selected at the moment");
                return;
            }
            // We want to ask for range, ev and equity of a current player to act.
            // If it is not action node - we will show those values for OOP Player
            IServerPlayer player;

            // We can get the action player from currently selected node
            if (currentlySelectedNode.ActionPlayer != null)
            {
                player = currentlySelectedNode.ActionPlayer;
            }
            else
            {
                // This is the way to get OOP player object from the API
                player = this.Context.ServerUtils.OOP;
            }

            // We will get the weight, ev and equity of the single holding AsQs,
            // average of all the holding in the AQ+ category
            // and total for all hands

            // We can construct a hand object by parsing single hand
            PioViewerApi.Util.Hand asqs = new Hand("AQss");
            // We can also parse range using utility method. Range object is a holder of hand, weight pairs
            PioViewerApi.Util.IRange aqplus = this.Context.ServerUtils.CreateRange("AQ+");

            //We will collect the information in the String Builder object.
            StringBuilder result = new StringBuilder();
            result.AppendLine("Basic report for " + player.Name + " in node " + currentlySelectedNode.NodeId);
            // This is how you convert a hand to nice looking string, and get the human readable format of a range
            result.AppendLine("Reporting range, EV and equity for " + asqs.ToUnicodeString() + " and subrange " + aqplus.ToSingleLineString());

            // Ask solver for the range of a given player in a node
            IRange range = this.Context.ServerWrapper.ShowRange(player, currentlySelectedNode);
            // Ask solver for equity
            EVRange equity = this.Context.ServerWrapper.CalcEquityInNode(player, currentlySelectedNode);
            // Ask solver for EV
            EVRange ev = this.Context.ServerWrapper.CalcEV(player, currentlySelectedNode);

            ReportRange(result, asqs, aqplus, range);
            result.AppendLine("Equity:");
            ReportEVEquity(result, equity, asqs, aqplus);
            result.AppendLine("EV:");
            ReportEVEquity(result, ev, asqs, aqplus);
            MessageBox.Show(result.ToString(), "Tutorial 4");
        }

        private void ReportEVEquity(StringBuilder result, EVRange equity, Hand hand, IRange manyHands)
        {
            // EVRange class, which is a holder for EV and Equity results contains for each hand values of
            // calculated EV, total number of matchups and total winnings. 
            // Except for some corner cases EV is equal Wins/ Matchups
            double handEV = equity[hand].EV;

            // Calculation of total EV for a given range we have to a be careful.
            // We cannot simply average p out the EV's, because different hands have different weights.
            // It is also not enough to simply weight them with the current range as this does not
            // take into account the card removal effect. The proper way is to calculated weighted EV
            // weighted by the number of different matchups the given hand will play.
            // To do this we will sum the total winnings of all hands in the range and divide it by the total
            // number of matchups played.
            double rangeWins = 0;
            double rangeMatchups = 0;
            foreach (var singleCombo in manyHands)
            {
                EVHolder ev = equity[singleCombo];
                // This is important consideration. Impossible hands are reported by the solver as 0 matchups and NaN Wins.
                // We don't want to add NaNs to the running sum so we need this check
                if (ev.Matchups > 0)
                {
                    rangeWins += ev.Wins;
                    rangeMatchups += ev.Matchups;
                }
            }

            //To calculate the total EV for all range we repeat the same process
            double totalWins = 0;
            double totalMatchups = 0;
            foreach (var singleCombo in equity)
            {
                EVHolder ev = equity[singleCombo];
                if (ev.Matchups > 0)
                {
                    totalWins += ev.Wins;
                    totalMatchups += ev.Matchups;
                }
            }

            result.AppendLine("for single hand  " + hand.ToUnicodeString() + " = " + handEV);
            result.AppendLine("for hands " + manyHands.ToSingleLineString() + " = " + (rangeWins / rangeMatchups));
            result.AppendLine("total = " + (totalWins / totalMatchups));

        }

        private void ReportRange(StringBuilder result, Hand hand, IRange manyHands, IRange range)
        {
            // This is how you get the weight of a hand from range
            double weightForSingleHand = range[hand];

            double totalCombinationsInSelectedSubRange = 0;
            // This is how you can iterate over all hands in a range
            foreach (Hand singleConbination in manyHands)
            {
                totalCombinationsInSelectedSubRange += manyHands[singleConbination];
            }

            //This is a shortcut to get the sum of all weights
            double totalWeightsInRange = range.TotalWeights();

            result.AppendLine("Weight for single hand  " + hand.ToUnicodeString() + " = " + weightForSingleHand);
            result.AppendLine("Summary weight for hands " + manyHands.ToSingleLineString() + " = " + totalCombinationsInSelectedSubRange);
            result.AppendLine("Total combos in range = " + totalWeightsInRange);
        }
    }

}
