import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/OpportunityManageCard.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:provider/provider.dart';

class OpportunityManagerScreen extends StatefulWidget {
  
  const OpportunityManagerScreen({super.key});

  @override
  _OpportunityManagerScreenState createState() => _OpportunityManagerScreenState();
}

class _OpportunityManagerScreenState extends State<OpportunityManagerScreen> {
  late Future<List<Opportunity>?> _opportunitiesFuture;

  @override
  void initState() {
    super.initState();
    _opportunitiesFuture = Provider.of<OpportunityApiHandler>(context, listen: false)
        .getAllOpportunitiesByUserId(1);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: FutureBuilder<List<Opportunity>?>(
        future: _opportunitiesFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(
              child: Text('Error loading opportunities: ${snapshot.error}'),
            );
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(child: Text('No opportunities found.'));
          } else {
            final opportunities = snapshot.data!;
            return LayoutBuilder(
              builder: (context, constraints) {
                if (constraints.maxWidth < 600) {
                  return _buildMobileLayout(opportunities);
                } else if (constraints.maxWidth < 1200) {
                  return _buildTabletLayout(opportunities);
                } else {
                  return _buildDesktopLayout(opportunities);
                }
              },
            );
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout(List<Opportunity> opportunities) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20.0),
      child: Column(
        children: [
          Text(
            "As suas Oportunidades",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          ...opportunities
              .map((opportunity) => Padding(
                    padding: const EdgeInsets.only(bottom: 20.0),
                    child: OpportunityManageCard(opportunity: opportunity),
                  ))
              .toList(),
        ],
      ),
    );
  }

  Widget _buildTabletLayout(List<Opportunity> opportunities) {
    return Scrollbar(
      thumbVisibility: true,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.all(20.0),
            child: Text(
              "As suas Oportunidades",
              style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
            ),
          ),
          SizedBox(height: 20),
          Expanded(
            child: GridView.builder(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2,
                crossAxisSpacing: 10,
                mainAxisSpacing: 20,
                childAspectRatio: 1.5,
              ),
              itemCount: opportunities.length,
              itemBuilder: (context, index) {
                return OpportunityManageCard(opportunity: opportunities[index]);
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDesktopLayout(List<Opportunity> opportunities) {
    return Scrollbar(
      thumbVisibility: true,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              "As suas Oportunidades",
              style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 20),
            GridView.builder(
              shrinkWrap: true,
              physics: NeverScrollableScrollPhysics(),
              gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 3,
                crossAxisSpacing: 20.0,
                mainAxisSpacing: 20.0,
              ),
              itemCount: opportunities.length,
              itemBuilder: (context, index) {
                return OpportunityManageCard(opportunity: opportunities[index]);
              },
            ),
          ],
        ),
      ),
    );
  }
}
