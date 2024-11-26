import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Location.dart';

class EditOpportunityScreen extends StatefulWidget {
  final Opportunity opportunity;

  const EditOpportunityScreen({super.key, required this.opportunity});

  @override
  _EditOpportunityScreenState createState() => _EditOpportunityScreenState();
}

class _EditOpportunityScreenState extends State<EditOpportunityScreen> {
  late ScrollController verticalScrollController;
  late TextEditingController nameController;
  late TextEditingController descriptionController;
  late TextEditingController addressController;
  late TextEditingController vacanciesController;

  late Location selectedLocation; // Use enum for location

  @override
  void initState() {
    super.initState();
    verticalScrollController = ScrollController();

    // Initialize controllers with the current values from the Opportunity object
    nameController = TextEditingController(text: widget.opportunity.name);
    descriptionController = TextEditingController(text: widget.opportunity.description);
    addressController = TextEditingController(text: widget.opportunity.address);
    vacanciesController = TextEditingController(text: widget.opportunity.vacancies.toString());
    selectedLocation = widget.opportunity.location; // Preselect the current location
  }

  @override
  void dispose() {
    verticalScrollController.dispose();
    nameController.dispose();
    descriptionController.dispose();
    vacanciesController.dispose();
    super.dispose();
  }

  void _saveOpportunity() {
    // Example: Save the opportunity along with the selected images, location, etc.
    Navigator.of(context).pop(widget.opportunity);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            return _buildTabletLayout();
          } else {
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return SingleChildScrollView(
      controller: verticalScrollController,
      padding: const EdgeInsets.all(20.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Editar a sua Oportunidade:",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          OpportunityTextField(
            label: 'Nome',
            controller: nameController,
          ),
          SizedBox(height: 20),
          OpportunityTextField(
            label: 'Descrição',
            controller: descriptionController,
            maxLines: 3,
          ),
          SizedBox(height: 20),
          OpportunityTextField(
            label: 'Endereço',
            controller: addressController,
          ),
          SizedBox(height: 20),
          Text(
            'Localização',
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 10),
          DropdownButton<Location>(
            isExpanded: true,
            value: selectedLocation,
            onChanged: (Location? newValue) {
              setState(() {
                selectedLocation = newValue!;
              });
            },
            items: Location.values.map((Location location) {
              return DropdownMenuItem<Location>(
                value: location,
                child: Text(location.name),
              );
            }).toList(),
          ),
          SizedBox(height: 20),
          OpportunityTextField(
            label: 'Vagas',
            controller: vacanciesController,
            maxLines: 1,
            inputType: TextInputType.number, // For numeric input
          ),
          SizedBox(height: 20),

          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Color(0xFF50C878), // Set button color here
            ),
            onPressed: _saveOpportunity,
            child: Text(
              'Salvar',
              style: TextStyle(color: Colors.white), // Adjust text color if needed
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTabletLayout() {
    return _buildMobileLayout(); // Can adjust the layout for larger screens if needed
  }

  Widget _buildDesktopLayout() {
    return _buildMobileLayout(); // Can adjust the layout for larger screens if needed
  }
}

// Reusable widget for text fields
class OpportunityTextField extends StatelessWidget {
  final String label;
  final TextEditingController controller;
  final int maxLines;
  final TextInputType? inputType;

  const OpportunityTextField({
    required this.label,
    required this.controller,
    this.maxLines = 1,
    this.inputType,
  });

  @override
  Widget build(BuildContext context) {
    return TextField(
      decoration: InputDecoration(
        labelText: label,
        border: OutlineInputBorder(),
      ),
      controller: controller,
      maxLines: maxLines,
      keyboardType: inputType,
    );
  }
}
